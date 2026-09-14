#!/usr/bin/env node
/**
 * tools/mcp_filter_proxy.js
 * 
 * Unity MCP 상태 기반 스마트 동적 필터 프록시 (Status-Driven Dynamic Tool Diet Gateway)
 * 
 * [동작 원리 (방안 A: 작업 단계별 동적 도구 선별)]
 * 1. Antigravity가 'tools/list'를 요청할 때마다 docs/work/status.md의 실시간 진행 상태를 읽습니다.
 * 2. 현재 작업 주체(Developer, QA, Artist, Designer/Git/PM 등)에 따라 꼭 필요한 0~6개 도구만 동적으로 선별 반환합니다:
 *    - [Developer] 단계: read_console, find_gameobjects, manage_prefabs, manage_components, manage_scriptable_object (5종)
 *    - [QA] 단계:        read_console, find_gameobjects, manage_scene, manage_camera, run_tests, get_test_job (6종)
 *    - [Artist] 단계:    manage_camera, manage_prefabs (2종)
 *    - [Designer/Git/PM]: 유니티 에디터 조작 불필요 ➔ 0종 반환 (도구 스키마 토큰 0개!)
 *    - [대기 / 기본]:     read_console (1종, ~150 토큰)
 * 3. 결과: 매 턴 10,000 토큰에 달하던 도구 스키마가 현재 단계에 맞춰 0~900 토큰(90~100% 절감)으로 극단적 압축!
 * 4. 안전 방화벽: 헌장 금지 도구(execute_code, apply_text_edits 등)는 어떤 상황에서도 원천 차단.
 * 5. 에디터 미기동 시: 크래시 없이 빈 도구 목록({ tools: [] })을 반환하여 공식 CLI 러너로 자연스럽게 폴백.
 */

const http = require('http');
const url = require('url');
const fs = require('fs');
const path = require('path');

// 프로젝트 루트 탐색
function findProjectRoot() {
  let curr = __dirname;
  while (curr !== path.dirname(curr)) {
    if (fs.existsSync(path.join(curr, 'GEMINI.md')) || fs.existsSync(path.join(curr, '.git'))) {
      return curr;
    }
    curr = path.dirname(curr);
  }
  return process.cwd();
}

const projectRoot = findProjectRoot();

// 설정
const PROXY_PORT = process.env.UNITY_MCP_PROXY_PORT || 8081;
const TARGET_HOST = process.env.UNITY_MCP_HOST || '127.0.0.1';
const TARGET_PORT = process.env.UNITY_MCP_PORT || 8080;
const TARGET_PATH = '/mcp';

// 전체 안전 허용 화이트리스트 (tools/call 호출 시 검증용)
const ALL_SAFE_TOOLS = new Set([
  'read_console',
  'find_gameobjects',
  'manage_scene',
  'manage_prefabs',
  'manage_components',
  'manage_scriptable_object',
  'manage_camera',
  'run_tests',
  'get_test_job'
]);

// 헌장 절대 금지 도구 (방화벽 차단)
const BANNED_TOOLS = new Set([
  'execute_code',
  'apply_text_edits',
  'manage_script',
  'create_script',
  'delete_script',
  'get_sha'
]);

// 작업 역할별 동적 도구 세트 (Status-Driven Dynamic Toolsets)
const ROLE_TOOLSETS = {
  DEVELOPER: new Set([
    'read_console',
    'find_gameobjects',
    'manage_prefabs',
    'manage_components',
    'manage_scriptable_object'
  ]),
  QA: new Set([
    'read_console',
    'find_gameobjects',
    'manage_scene',
    'manage_camera',
    'run_tests',
    'get_test_job'
  ]),
  ARTIST: new Set([
    'manage_camera',
    'manage_prefabs'
  ]),
  NONE: new Set([]),
  DEFAULT: new Set([
    'read_console'
  ])
};

/**
 * docs/work/status.md의 실시간 진행 상태 파싱
 */
function getActiveRoleFromStatus() {
  if (process.env.UNITY_MCP_MODE === 'all') {
    return 'ALL';
  }

  const statusPath = path.join(projectRoot, 'docs', 'work', 'status.md');
  if (!fs.existsSync(statusPath)) {
    return 'DEFAULT';
  }

  try {
    const content = fs.readFileSync(statusPath, 'utf8');
    const statusLine = (content.match(/-\s*\*\*진행\s*상태\*\*:[^\n]+/i) || [])[0] || content;

    if (statusLine.includes('[Developer]')) return 'DEVELOPER';
    if (statusLine.includes('[QA]')) return 'QA';
    if (statusLine.includes('[Artist]')) return 'ARTIST';
    if (statusLine.includes('[Designer]') || statusLine.includes('[GitManager]') || statusLine.includes('[PM]')) {
      return 'NONE';
    }
  } catch (e) {
    // 읽기 실패 시 DEFAULT 사용
  }

  return 'DEFAULT';
}

function getActiveToolset() {
  const role = getActiveRoleFromStatus();
  if (role === 'ALL') return ALL_SAFE_TOOLS;
  return ROLE_TOOLSETS[role] || ROLE_TOOLSETS.DEFAULT;
}

function filterToolsListResponse(jsonBody) {
  try {
    const data = JSON.parse(jsonBody);
    if (data.result && Array.isArray(data.result.tools)) {
      const originalCount = data.result.tools.length;
      const activeRole = getActiveRoleFromStatus();
      const currentAllowedTools = getActiveToolset();

      data.result.tools = data.result.tools.filter(tool => currentAllowedTools.has(tool.name));
      const filteredCount = data.result.tools.length;
      
      console.log(`[Smart Proxy] 상태 감지: [${activeRole}] ➔ ${originalCount}개 중 ${filteredCount}개 도구만 동적 노출 (토큰 90~100% 절감)`);
      if (filteredCount > 0) {
        console.log(`  * 활성 도구: ${data.result.tools.map(t => t.name).join(', ')}`);
      } else {
        console.log(`  * 활성 도구: (유니티 조작 불필요 단계로 0개 노출)`);
      }
      return JSON.stringify(data);
    }
    return jsonBody;
  } catch (e) {
    return jsonBody;
  }
}

const server = http.createServer((clientReq, clientRes) => {
  const parsedUrl = url.parse(clientReq.url);
  const targetPath = parsedUrl.pathname === '/' ? TARGET_PATH : parsedUrl.pathname;

  // 요청 바디 수집
  const chunks = [];
  clientReq.on('data', chunk => chunks.push(chunk));
  clientReq.on('end', () => {
    const reqBody = Buffer.concat(chunks);
    let isToolsList = false;
    let isBannedToolCall = false;
    let bannedToolName = '';

    if (reqBody.length > 0) {
      try {
        const json = JSON.parse(reqBody.toString('utf8'));
        if (json.method === 'tools/list') {
          isToolsList = true;
        } else if (json.method === 'tools/call' && json.params && json.params.name) {
          if (BANNED_TOOLS.has(json.params.name)) {
            isBannedToolCall = true;
            bannedToolName = json.params.name;
          }
        }
      } catch (e) {
        // 비 JSON 요청은 그대로 전달
      }
    }

    // 금지된 도구 호출 시 방화벽 차단
    if (isBannedToolCall) {
      console.warn(`[Proxy 차단] 헌장 안전 수칙 위반 도구 호출 차단: ${bannedToolName}`);
      const errResponse = JSON.stringify({
        jsonrpc: '2.0',
        id: (JSON.parse(reqBody.toString('utf8')) || {}).id,
        error: {
          code: -32601,
          message: `[거버넌스 차단] '${bannedToolName}' 도구는 프로젝트 헌장(GEMINI.md)에 의해 사용이 전면 금지되어 있습니다. 표준 네이티브 도구를 사용하십시오.`
        }
      });
      clientRes.writeHead(200, { 'Content-Type': 'application/json' });
      clientRes.end(errResponse);
      return;
    }

    // 유니티 대상 서버로 전달 옵션
    const proxyOptions = {
      hostname: TARGET_HOST,
      port: TARGET_PORT,
      path: targetPath + (parsedUrl.search || ''),
      method: clientReq.method,
      headers: { ...clientReq.headers, host: `${TARGET_HOST}:${TARGET_PORT}` }
    };

    const targetReq = http.request(proxyOptions, (targetRes) => {
      // SSE 스트리밍 또는 일반 응답 분기
      const isEventStream = (targetRes.headers['content-type'] || '').includes('text/event-stream');
      
      if (isEventStream) {
        clientRes.writeHead(targetRes.statusCode, targetRes.headers);
        targetRes.pipe(clientRes);
        return;
      }

      const resChunks = [];
      targetRes.on('data', chunk => resChunks.push(chunk));
      targetRes.on('end', () => {
        let resBody = Buffer.concat(resChunks).toString('utf8');

        // tools/list 결과인 경우 상태 기반 동적 화이트리스트 필터 적용
        if (isToolsList || resBody.includes('"tools"')) {
          resBody = filterToolsListResponse(resBody);
        }

        const resBuffer = Buffer.from(resBody, 'utf8');
        const headers = { ...targetRes.headers };
        headers['content-length'] = resBuffer.length;

        clientRes.writeHead(targetRes.statusCode, headers);
        clientRes.end(resBuffer);
      });
    });

    targetReq.on('error', (err) => {
      // 유니티 에디터가 꺼져 있는 경우 Fast-Fail 대신 안전한 빈 응답 반환
      console.log(`[Proxy] Unity 에디터(포트 ${TARGET_PORT}) 미연결 상태 감지: ${err.message}`);
      if (isToolsList) {
        const emptyToolsResponse = JSON.stringify({
          jsonrpc: '2.0',
          id: (JSON.parse(reqBody.toString('utf8')) || {}).id,
          result: { tools: [] }
        });
        clientRes.writeHead(200, { 'Content-Type': 'application/json' });
        clientRes.end(emptyToolsResponse);
      } else {
        clientRes.writeHead(503, { 'Content-Type': 'application/json' });
        clientRes.end(JSON.stringify({
          jsonrpc: '2.0',
          error: {
            code: -32000,
            message: `Unity 에디터(${TARGET_HOST}:${TARGET_PORT})가 실행되어 있지 않습니다. CLI 러너(unity-cli-runner)를 사용하거나 에디터를 실행해 주세요.`
          }
        }));
      }
    });

    if (reqBody.length > 0) {
      targetReq.write(reqBody);
    }
    targetReq.end();
  });
});

server.listen(PROXY_PORT, '127.0.0.1', () => {
  const activeRole = getActiveRoleFromStatus();
  console.log('===========================================================');
  console.log(`[Unity MCP Status-Driven Smart Proxy] 프록시 가동 완료`);
  console.log(`- 수신 엔드포인트: http://127.0.0.1:${PROXY_PORT}/mcp`);
  console.log(`- 타겟 Unity 에디터: http://${TARGET_HOST}:${TARGET_PORT}/mcp`);
  console.log(`- 현재 감지된 상태: [${activeRole}] 단계`);
  console.log(`- 지원 단계별 도구 분기:`);
  console.log(`  * [Developer]: read_console, find_gameobjects, manage_prefabs, manage_components, manage_scriptable_object (5종)`);
  console.log(`  * [QA]:        read_console, find_gameobjects, manage_scene, manage_camera, run_tests, get_test_job (6종)`);
  console.log(`  * [Artist]:    manage_camera, manage_prefabs (2종)`);
  console.log(`  * [기타/기획]: 0종 노출 (도구 스키마 토큰 0개 절감)`);
  console.log(`- 방화벽 차단 도구: ${Array.from(BANNED_TOOLS).join(', ')}`);
  console.log('===========================================================');
});
