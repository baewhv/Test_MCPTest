const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

function findProjectRoot() {
  let curr = __dirname;
  while (curr !== path.dirname(curr)) {
    if (fs.existsSync(path.join(curr, 'GEMINI.md')) || fs.existsSync(path.join(curr, '.git'))) {
      return curr;
    }
    curr = path.dirname(curr);
  }
  return path.resolve(__dirname, '../../../../..');
}

const projectRoot = findProjectRoot();
const logsDir = path.join(projectRoot, 'Logs');

/**
 * docs/PROJECT_SPEC.md 파싱 유틸리티
 */
function getProjectSpec() {
  const specPath = path.join(projectRoot, 'docs', 'PROJECT_SPEC.md');
  const spec = {
    projectName: '',
    unityVersion: '',
    unityEditorPath: ''
  };

  if (!fs.existsSync(specPath)) {
    return spec;
  }

  const content = fs.readFileSync(specPath, 'utf8');
  const nameMatch = content.match(/Unity Project Name\s*:\s*`?([^`\r\n]+)`?/i);
  if (nameMatch && nameMatch[1].trim()) spec.projectName = nameMatch[1].trim();

  const versionMatch = content.match(/Unity Version\s*:\s*`?([^`\r\n]+)`?/i);
  if (versionMatch && versionMatch[1].trim()) spec.unityVersion = versionMatch[1].trim();

  const pathMatch = content.match(/Unity Editor Path\s*:\s*`?([^`\r\n]+)`?/i);
  if (pathMatch && pathMatch[1].trim()) spec.unityEditorPath = pathMatch[1].trim();

  return spec;
}

/**
 * 워크스페이스(OS) 내에 지정된 버전의 Unity.exe가 설치되어 있는지 탐색 및 검증
 */
function findUnityEditor(specifiedVersion, specifiedPath) {
  // 1. 직접 명시된 경로가 유효한지 우선 확인
  if (specifiedPath && fs.existsSync(specifiedPath)) {
    return specifiedPath;
  }

  const hubEditorBase = 'C:\\Program Files\\Unity\\Hub\\Editor';

  // 2. 버전이 명시되어 있는 경우 정확한 경로 확인
  if (specifiedVersion) {
    const candidate = path.join(hubEditorBase, specifiedVersion, 'Editor', 'Unity.exe');
    if (fs.existsSync(candidate)) {
      return candidate;
    }
  }

  // 3. Unity Hub 디렉토리에서 설치된 버전 탐색
  if (fs.existsSync(hubEditorBase)) {
    const installed = fs.readdirSync(hubEditorBase);
    if (installed.length > 0) {
      if (specifiedVersion) {
        const matched = installed.find(v => v.includes(specifiedVersion));
        if (matched) {
          const matchedPath = path.join(hubEditorBase, matched, 'Editor', 'Unity.exe');
          if (fs.existsSync(matchedPath)) return matchedPath;
        }
      } else {
        // 버전 미지정 시 설치된 첫 번째 에디터 사용
        const fallbackPath = path.join(hubEditorBase, installed[0], 'Editor', 'Unity.exe');
        if (fs.existsSync(fallbackPath)) return fallbackPath;
      }
    }
  }

  return null;
}

/**
 * Unity 프로젝트 설치 상태 진단 (check)
 */
function checkInstallation() {
  const spec = getProjectSpec();
  const projectVersionFile = path.join(projectRoot, 'ProjectSettings', 'ProjectVersion.txt');
  console.log('==================================================');
  console.log('[Unity CLI Setup] Unity 프로젝트 및 환경 진단');
  console.log('==================================================');
  console.log(`- 대상 프로젝트 루트: ${projectRoot}`);
  console.log(`- Project Name (spec): ${spec.projectName || '(미지정)'}`);
  console.log(`- Unity Version (spec): ${spec.unityVersion || '(미지정)'}`);

  const isInstalled = fs.existsSync(projectVersionFile);
  if (isInstalled) {
    const versionContent = fs.readFileSync(projectVersionFile, 'utf8');
    const installedVer = (versionContent.match(/m_EditorVersion:\s*(.+)/) || [])[1] || 'Unknown';
    console.log(`- 현재 설치 상태: 설치 완료됨 (설치된 버전: ${installedVer})`);
  } else {
    console.log(`- 현재 설치 상태: 미설치 (ProjectSettings/ProjectVersion.txt 부재)`);
  }

  const unityPath = findUnityEditor(spec.unityVersion, spec.unityEditorPath);
  if (unityPath) {
    console.log(`- 워크스페이스 Unity Editor: 사용 가능 (${unityPath})`);
  } else {
    console.log(`- 워크스페이스 Unity Editor: 미발견 (지정 버전: ${spec.unityVersion})`);
  }
  console.log('==================================================');
  return { isInstalled, unityPath, spec };
}

/**
 * Unity 프로젝트 신규 생성/설치 (init)
 */
function initProject() {
  const { isInstalled, unityPath, spec } = checkInstallation();

  // 1. 이미 설치되어 있는지 확인
  if (isInstalled) {
    console.log(`[Unity CLI Setup] 이미 프로젝트 최상단에 Unity 프로젝트 환경이 구축되어 있습니다.`);
    console.log(`[Unity CLI Setup] 기존 프로젝트를 보존하며 생성을 건너뜁니다.`);
    return true;
  }

  // 2. Unity Editor 설치 여부 확인
  if (!unityPath) {
    console.error(`[Unity CLI Setup 오류] 지정된 Unity 버전(${spec.unityVersion || '미지정'})의 실행 파일을 찾을 수 없습니다.`);
    console.error(`- 해결 방안: Unity Hub에서 해당 버전을 설치하거나 docs/PROJECT_SPEC.md의 버전을 일치시켜 주세요.`);
    process.exit(1);
  }

  // 3. Unity -createProject 실행하여 프로젝트 최상단에 설치
  console.log(`[Unity CLI Setup] 프로젝트 최상단에 신규 Unity 프로젝트 생성을 개시합니다...`);
  console.log(`- 대상 경로: ${projectRoot}`);
  console.log(`- 실행 에디터: ${unityPath}`);

  if (!fs.existsSync(logsDir)) {
    fs.mkdirSync(logsDir, { recursive: true });
  }
  const logFile = path.join(logsDir, 'Unity_CreateProject.log');

  try {
    const cmd = `"${unityPath}" -batchmode -createProject "${projectRoot}" -quit -logFile "${logFile}"`;
    console.log(`- 실행 명령: ${cmd}`);
    execSync(cmd, { stdio: 'inherit', timeout: 300000 });

    const projectVersionFile = path.join(projectRoot, 'ProjectSettings', 'ProjectVersion.txt');
    if (fs.existsSync(projectVersionFile)) {
      console.log(`[Unity CLI Setup] Unity 프로젝트가 성공적으로 최상단에 개설되었습니다!`);
      return true;
    } else {
      console.error(`[Unity CLI Setup 오류] 생성이 완료되었으나 ProjectSettings/ProjectVersion.txt를 확인할 수 없습니다.`);
      console.error(`- 로그 파일: ${logFile}`);
      process.exit(1);
    }
  } catch (err) {
    console.error(`[Unity CLI Setup 오류] 프로젝트 생성 중 오류가 발생했습니다: ${err.message}`);
    process.exit(1);
  }
}

// CLI 명령 라우팅
const args = process.argv.slice(2);
const command = args[0] || 'check';

switch (command) {
  case 'init':
  case 'create':
    initProject();
    break;
  case 'check':
    checkInstallation();
    break;
  default:
    console.log('사용법: node unity_cli.js [init | check]');
    console.log('  - init : docs/PROJECT_SPEC.md를 기반으로 Unity 프로젝트를 최상단에 개설/설치합니다.');
    console.log('  - check: 현재 Unity 설치 상태 및 버전 일치 여부를 진단합니다.');
}
