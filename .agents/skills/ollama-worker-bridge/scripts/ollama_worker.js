const http = require('http');
const fs = require('fs');
const path = require('path');

const args = process.argv.slice(2);
const params = {};
for (let i = 0; i < args.length; i++) {
  if (args[i].startsWith('--')) {
    const key = args[i].substring(2);
    const value = args[i + 1] && !args[i + 1].startsWith('--') ? args[i + 1] : true;
    params[key] = value;
    if (value !== true) i++;
  }
}

const model = params.model || 'codellama:7b';
const worker = params.worker || (model.includes('llama') ? 'code_worker' : 'docs_worker');
const desc = params.desc || 'Ollama 연산 작업';
let prompt = params.prompt || '';
const timeoutMs = parseInt(params.timeout || '300000', 10);

if (fs.existsSync(prompt)) {
  prompt = fs.readFileSync(prompt, 'utf8');
}

if (!prompt) {
  console.error('Error: --prompt parameter is required.');
  process.exit(1);
}

function getTodayString() {
  const now = new Date();
  const yyyy = now.getFullYear();
  const mm = String(now.getMonth() + 1).padStart(2, '0');
  const dd = String(now.getDate()).padStart(2, '0');
  return `${yyyy}-${mm}-${dd}`;
}

function logToTable(workerName, modelName, elapsedSec, description) {
  const today = getTodayString();
  const logDir = path.resolve(__dirname, '../../../../docs/logs');
  if (!fs.existsSync(logDir)) {
    fs.mkdirSync(logDir, { recursive: true });
  }

  const logFile = path.join(logDir, `worker_log_${today}.md`);
  let content = '';
  let nextIndex = 1;

  if (fs.existsSync(logFile)) {
    content = fs.readFileSync(logFile, 'utf8');
    const lines = content.trim().split('\n');
    for (let i = lines.length - 1; i >= 0; i--) {
      const match = lines[i].match(/^\|\s*(\d+)\s*\|/);
      if (match) {
        nextIndex = parseInt(match[1], 10) + 1;
        break;
      }
    }
  } else {
    content = `# 통합 워커 작업 기록 (${today})\n\n| Index | 작업자 | AI 모델 | 소요 시간 | 대략적인 내용 |\n| :--- | :--- | :--- | :--- | :--- |\n`;
  }

  const newRow = `| ${nextIndex} | ${workerName} | ${modelName} | ${elapsedSec}초 | ${description} |\n`;
  content += newRow;
  fs.writeFileSync(logFile, content, 'utf8');
  console.error(`[로깅 완료] ${logFile} (Index: ${nextIndex}, 소요 시간: ${elapsedSec}초)`);
}

const postData = JSON.stringify({
  model: model,
  prompt: prompt,
  stream: false,
  keep_alive: '30m'
});

const startTime = Date.now();
console.error(`[시작] ${worker} -> ${model} 요청 전송 중... (타임아웃: ${timeoutMs / 1000}초)`);

const req = http.request({
  hostname: 'baewhv.iptime.org',
  port: 11435,
  path: '/api/generate',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Content-Length': Buffer.byteLength(postData),
    'Authorization': 'Bearer sk-master-ollama-key'
  },
  timeout: timeoutMs
}, (res) => {
  let resBody = '';
  res.on('data', chunk => resBody += chunk);
  res.on('end', () => {
    const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
    if (res.statusCode >= 200 && res.statusCode < 300) {
      try {
        const parsed = JSON.parse(resBody);
        logToTable(worker, model, elapsed, desc);
        process.stdout.write(parsed.response || '');
      } catch (err) {
        console.error(`[오류] JSON 파싱 실패: ${err.message}`);
        process.exit(1);
      }
    } else {
      console.error(`[오류] HTTP 상태 코드: ${res.statusCode} - ${resBody}`);
      process.exit(1);
    }
  });
});

req.on('error', (e) => {
  const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
  console.error(`[네트워크 오류] ${e.message} (소요 시간: ${elapsed}초)`);
  process.exit(1);
});

req.on('timeout', () => {
  const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
  req.destroy();
  console.error(`[타임아웃 오류] ${model} 5분 초과로 요청 중단 (소요 시간: ${elapsed}초)`);
  process.exit(1);
});

req.write(postData);
req.end();
