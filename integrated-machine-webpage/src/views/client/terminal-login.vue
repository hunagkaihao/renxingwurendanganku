<template>
  <div class="sample-login-page">
    <header class="sample-login-header">
      <button type="button" class="sample-login-back" @click="back"><Icon type="ios-arrow-back" /> 返回操作台</button>
      <span>智慧无人库 · 身份验证</span>
    </header>
    <main class="sample-login-main">
      <section class="sample-login-card" aria-labelledby="sample-login-title">
        <span class="sample-login-badge">{{ operationLabel }}</span>
        <h1 id="sample-login-title">{{ registrationLogin ? '请使用管理员账号登录' : '请登录后继续' + operationLabel }}</h1>
        <p class="sample-login-description">{{ registrationLogin ? '验证管理员身份后，可新增人员或录入已有人员的特征' : '请选择一种登录方式完成身份验证' }}</p>
        <div v-if="!registrationLogin" class="sample-login-methods" role="tablist" aria-label="登录方式">
          <button v-for="item in methods" :key="item.value" type="button" role="tab"
            :id="'login-tab-' + item.value" aria-controls="sample-login-panel"
            :aria-selected="method === item.value" :class="{ active: method === item.value }"
            :disabled="submitting" @click="changeMethod(item.value)">
            <Icon :type="item.icon" size="26" /><span>{{ item.label }}</span>
          </button>
        </div>
        <div id="sample-login-panel" class="sample-login-panel" role="tabpanel" :aria-labelledby="'login-tab-' + method">
          <form v-if="method === 'password'" autocomplete="off" @submit.prevent="submit">
            <label for="terminal-account">账号</label>
            <input id="terminal-account" ref="account" v-model="account" autocomplete="off" :readonly="!credentialsEditable.account"
              :disabled="submitting" placeholder="请输入 WMS 账号" @focus="activateInput('account')">
            <label for="terminal-password">密码</label>
            <input id="terminal-password" ref="password" v-model="password" type="password" autocomplete="new-password" :readonly="!credentialsEditable.password"
              :disabled="submitting" placeholder="请输入密码" @focus="activateInput('password')">
            <div class="sample-login-keyboard"><span>使用账号和密码验证身份</span>
              <button type="button" :disabled="submitting" @click="openKeyboard"><Icon type="ios-keypad-outline" /> 打开键盘</button>
            </div>
            <button type="submit" class="sample-login-submit" :disabled="submitting || !destination">
              {{ submitting ? '正在登录…' : '登录并继续' }}
            </button>
          </form>
          <div v-else-if="method === 'face'" class="sample-login-device">
            <label for="terminal-camera">外接摄像头</label>
            <div class="sample-login-camera-controls">
              <select id="terminal-camera" v-model="cameraDeviceId" :disabled="cameraOpening || submitting" @change="changeCamera">
                <option value="" disabled>请选择外接摄像头</option>
                <option v-for="(device, index) in cameraDevices" :key="device.deviceId" :value="device.deviceId">
                  {{ (device.label || '摄像头（授权后显示名称）') + ' · ' + (index + 1) }}
                </option>
              </select>
              <button type="button" :disabled="cameraOpening || submitting" @click="refreshCameras">刷新设备</button>
            </div>
            <div v-if="cameraNeedsAuthorization" class="sample-login-camera-authorization">
              <p>读取完整设备列表可能短暂打开浏览器默认摄像头，列出设备后立即关闭，不用于预览或拍照。</p>
              <button type="button" :disabled="cameraOpening || submitting" @click="authorizeCameras">{{ cameraPermissionState === 'granted' ? '初始化并刷新设备' : '授权并刷新设备' }}</button>
              <p>如不希望打开默认摄像头，请在浏览器地址栏的站点设置中允许摄像头，并在浏览器摄像头设置中选择外接 RGB Camera，再点击刷新设备。</p>
            </div>
            <p v-if="cameraNeedsAuthorization && cameraNotice" class="sample-login-camera-notice" role="status">{{ cameraNotice }}</p>
            <div class="sample-login-camera">
              <video ref="camera" autoplay playsinline muted @loadeddata="cameraReady = true"></video>
              <span v-if="!cameraReady">{{ cameraOpening ? '正在开启摄像头…' : '请开启摄像头' }}</span>
              <svg class="sample-login-face-frame" viewBox="0 0 180 240" aria-hidden="true" focusable="false">
                <path d="M90 8 C48 8 20 31 18 74 C15 122 29 175 54 211 C65 227 77 234 90 234 C103 234 115 227 126 211 C151 175 165 122 162 74 C160 31 132 8 90 8 Z" />
              </svg>
            </div>
            <p>请正对摄像头，保持面部清晰无遮挡</p>
            <button v-if="!cameraReady" type="button" class="sample-login-submit" :disabled="cameraOpening || submitting" @click="openCamera">开启摄像头</button>
            <button v-else type="button" class="sample-login-submit" :disabled="submitting || !destination" @click="submit">
              {{ submitting ? '正在识别人脸…' : '识别人脸并登录' }}
            </button>
          </div>
          <div v-else class="sample-login-device">
            <div class="sample-login-finger"><img src="../../images/finger.png" alt="指静脉采集示意"></div>
            <p>{{ submitting ? '正在等待采集，请保持手指平稳' : '请将已录入的手指放在指静脉识别区域' }}</p>
            <button type="button" class="sample-login-submit" :disabled="submitting || !destination" @click="submit">
              {{ submitting ? '正在识别指静脉…' : '识别指静脉并登录' }}
            </button>
          </div>
        </div>
        <p v-if="errorMessage" class="sample-login-error" role="alert">{{ errorMessage }}</p>
        <p class="sample-login-tip" aria-live="polite">{{ submitting ? '身份验证中，请稍候；返回操作台可取消本次登录。' : (registrationLogin ? '仅管理员可进行人员注册，退出注册页面后将清除本次管理员会话。' : '验证成功后，将继续您刚才选择的业务。') }}</p>
      </section>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Watch } from 'vue-property-decorator';
import axios, { CancelTokenSource } from 'axios';
import Cookies from 'js-cookie';
import AbpBase from '../../lib/abpbase';
import Util from '../../lib/util';
import AppConsts from '../../lib/appconst';
import { passwordLogin, biometricLogin, sampleDestinations, terminalLoginError } from '../../lib/terminal-auth';
import { CameraPermissionState, readCameraPermissionState, inspectExternalCameras, authorizeCameraDevices, selectExternalCamera, externalCameraConstraints, terminalCameraError } from '../../lib/terminal-camera';
import { isTerminalAdmin, checkRegistrationAccess, registrationError } from '../../lib/terminal-registration';

@Component
export default class TerminalLogin extends AbpBase {
  /** 三种互相独立的身份验证入口。 */
  methods = [
    { value: 'password', label: '账号密码', icon: 'ios-lock-outline' },
    { value: 'face', label: '人脸识别', icon: 'ios-camera-outline' },
    { value: 'vein', label: '指静脉', icon: 'ios-finger-print' }
  ];
  /** 当前选择的方式，默认使用不依赖设备的账号密码。 */
  method: string = 'password';
  /** 本次输入的 WMS 账号，仅保留在页面内存。 */
  account: string = '';
  /** 本次输入的密码，离开页面后清除。 */
  password: string = '';
  /** 输入框获得人工焦点后才允许编辑，配合关闭自动完成，减少浏览器在进入页面时回填已保存凭据。 */
  credentialsEditable = { account: false, password: false };
  /** 识别或会话初始化期间禁止重复提交。 */
  submitting: boolean = false;
  /** 可展示给使用人的登录或设备提示。 */
  errorMessage: string = '';
  /** 摄像头已经输出可拍摄画面。 */
  cameraReady: boolean = false;
  /** 防止重复申请摄像头权限。 */
  cameraOpening: boolean = false;
  /** 浏览器可见且未明确标识为内置摄像头的视频设备；同名设备保留为独立选项。 */
  cameraDevices: MediaDeviceInfo[] = [];
  /** 用户选定的外接摄像头 ID，只用于当前页面，不允许失败后回退到默认设备。 */
  cameraDeviceId: string = '';
  /** 浏览器未提供完整视频设备信息时，显示用户主动触发的授权入口。 */
  cameraNeedsAuthorization: boolean = false;
  /** 摄像头授权状态或双目通道选择说明。 */
  cameraNotice: string = '';
  /** 当前站点的摄像头权限；不支持查询时为 unknown。 */
  cameraPermissionState: CameraPermissionState = 'unknown';
  /** 打开系统键盘前恢复焦点的输入框。 */
  activeInput: string = 'account';
  /** 本页面持有的摄像头流，切换方式或离开页面时释放。 */
  private stream: MediaStream | null = null;
  /** 取消当前 HTTP 登录请求。 */
  private request: CancelTokenSource | null = null;
  /** 页面和异步操作代次，防止离开页面后的识别响应继续登录。 */
  private generation: number = 0;
  /** 登录流程成功跳转后，销毁组件时保留已经建立的会话。 */
  private completed: boolean = false;

  /** 仅接受本次业务允许的路由名称，刷新页面仍能继续同一业务。 */
  get destination(): string {
    const next = this.$route.query.next;
    return typeof next === 'string' && (sampleDestinations.indexOf(next) >= 0 || next === 'userregister') ? next : '';
  }

  /** 人员注册只能通过本轮管理员账号密码验证进入。 */
  get registrationLogin(): boolean { return this.destination === 'userregister'; }

  /** 根据原始留样、取样入口展示当前操作名称。 */
  get operationLabel(): string {
    if (this.registrationLogin) return '人员注册';
    return this.destination.indexOf('stockout') === 0 ? '取样' : '留样';
  }

  /** 开始新的终端身份验证，不复用上一人的登录身份。 */
  created() {
    this.resetCredentials();
    this.clearSession();
    if (!this.destination) this.errorMessage = '请返回操作台，重新选择留样或取样。';
  }

  /** 每次切换业务入口都从空账号、空密码开始，兼容 Vue 复用同一登录组件的情况。 */
  @Watch('$route.query.next')
  resetCredentials() {
    this.account = '';
    this.password = '';
    this.credentialsEditable = { account: false, password: false };
  }

  /** 用户点击或使用屏幕键盘聚焦后开放对应输入框，不读取浏览器或本地存储中的凭据。 */
  activateInput(input: string) {
    this.activeInput = input;
    if (input === 'account') this.credentialsEditable.account = true;
    if (input === 'password') this.credentialsEditable.password = true;
  }

  /** 选择登录方式时清理摄像头和此前的提示，不自动触发识别。 */
  async changeMethod(method: string) {
    if (this.registrationLogin && method !== 'password') return;
    if (this.submitting || this.method === method) return;
    this.generation++;
    this.closeCamera();
    this.method = method;
    this.resetCredentials();
    this.errorMessage = '';
    if (method === 'face') {
      await this.$nextTick();
      this.openCamera();
    }
  }

  /** 刷新外接候选设备；权限尚未授予时名称可能为空，需用户明确选择设备。 */
  async refreshCameras() {
    if (this.cameraOpening || this.submitting || this.method !== 'face') return;
    const generation = ++this.generation;
    this.closeCamera();
    this.errorMessage = '';
    this.cameraOpening = true;
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
        this.errorMessage = '当前浏览器无法列出摄像头，请通过 localhost、127.0.0.1 或 HTTPS 访问。';
        return;
      }
      const [devices, permission] = await Promise.all([
        navigator.mediaDevices.enumerateDevices(), readCameraPermissionState(navigator.permissions)
      ]);
      if (generation !== this.generation || this.method !== 'face') return;
      this.cameraPermissionState = permission;
      this.updateCameraDevices(devices);
      if (this.cameraDevices.length && !this.cameraDeviceId && !this.cameraNeedsAuthorization) {
        this.errorMessage = '请选择外接摄像头；首次授权前名称可能不可见，请在浏览器授权时确认外接设备。';
      }
    } catch (error) {
      if (generation === this.generation) this.errorMessage = terminalCameraError(error);
    } finally {
      if (generation === this.generation) this.cameraOpening = false;
    }
  }

  /** 主动申请摄像头权限，临时默认流只用来列举设备，随后关闭并等待用户选择外接设备。 */
  async authorizeCameras() {
    if (this.cameraOpening || this.submitting || this.method !== 'face') return;
    const generation = ++this.generation;
    this.closeCamera();
    this.cameraOpening = true;
    this.errorMessage = '';
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia)
        throw new Error('请通过 localhost、127.0.0.1 或 HTTPS 访问并允许摄像头权限。');
      const devices = await authorizeCameraDevices(navigator.mediaDevices,
        () => generation === this.generation && this.method === 'face', stream => { this.stream = stream; });
      const permission = await readCameraPermissionState(navigator.permissions);
      if (generation !== this.generation || this.method !== 'face') return;
      this.cameraPermissionState = permission;
      this.updateCameraDevices(devices);
    } catch (error) {
      if (generation === this.generation)
        this.errorMessage = error.name === 'Error' ? error.message : terminalCameraError(error);
    } finally {
      if (generation === this.generation) { this.closeCamera(); this.cameraOpening = false; }
    }
  }

  /** 按完整列表解释权限状态；已有视频流时禁止自动换选其他设备，确保预览和选择一致。 */
  private updateCameraDevices(devices: MediaDeviceInfo[], allowAutoSelect: boolean = true) {
    const discovery = inspectExternalCameras(devices, this.cameraPermissionState);
    this.cameraDevices = discovery.devices;
    this.cameraNeedsAuthorization = discovery.needsAuthorization;
    this.cameraNotice = discovery.message;
    if (!this.cameraDevices.some(device => device.deviceId === this.cameraDeviceId))
      this.cameraDeviceId = allowAutoSelect ? selectExternalCamera(this.cameraDevices, '') : '';
  }

  /** 切换设备前释放旧流，随后只申请当前选定的摄像头。 */
  changeCamera() {
    if (this.cameraOpening || this.submitting) return;
    this.generation++;
    this.closeCamera();
    this.openCamera();
  }

  /** 精确申请所选外接摄像头，权限请求延迟完成时也只允许当前页面持有该流。 */
  async openCamera() {
    if (this.cameraOpening || this.stream || this.method !== 'face') return;
    if (!this.cameraDeviceId) {
      await this.refreshCameras();
      if (!this.cameraDeviceId || this.method !== 'face' || this.cameraOpening || this.stream) return;
    }
    const generation = this.generation;
    this.cameraOpening = true;
    this.errorMessage = '';
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        this.errorMessage = '当前浏览器无法使用摄像头，请在本机或 HTTPS 页面访问，或切换账号密码登录。';
        return;
      }
      const stream = await navigator.mediaDevices.getUserMedia(externalCameraConstraints(this.cameraDeviceId));
      if (generation !== this.generation || this.method !== 'face') {
        stream.getTracks().forEach(track => track.stop());
        return;
      }
      this.stream = stream;
      // 授权后重新获取名称，避免保留匿名设备选项；不改变用户选择的设备。
      const [devices, permission] = await Promise.all([
        navigator.mediaDevices.enumerateDevices(), readCameraPermissionState(navigator.permissions)
      ]);
      if (generation !== this.generation || this.method !== 'face') return;
      this.cameraPermissionState = permission;
      this.updateCameraDevices(devices, false);
      if (!this.cameraDevices.some(device => device.deviceId === this.cameraDeviceId)) {
        this.closeCamera();
        this.cameraDeviceId = '';
        this.errorMessage = '当前设备不是可用的外接摄像头，请重新选择外接设备。';
        return;
      }
      const camera = this.$refs.camera as HTMLVideoElement;
      camera.srcObject = stream;
      await camera.play();
    } catch (error) {
      if (generation === this.generation) {
        this.closeCamera();
        if (error.name === 'NotAllowedError' || error.name === 'PermissionDeniedError') this.cameraNeedsAuthorization = true;
        this.errorMessage = terminalCameraError(error);
      }
    } finally {
      if (generation === this.generation) this.cameraOpening = false;
    }
  }

  /** 释放所有音视频轨道并停止本页预览。 */
  private closeCamera() {
    if (this.stream) this.stream.getTracks().forEach(track => track.stop());
    this.stream = null;
    this.cameraReady = false;
    this.cameraOpening = false;
    const camera = this.$refs.camera as HTMLVideoElement;
    if (camera) camera.srcObject = null;
  }

  /** 使用一体机现有屏幕键盘桥接；普通浏览器仍可使用系统键盘。 */
  openKeyboard() {
    const input = this.$refs[this.activeInput] as HTMLInputElement;
    if (input) input.focus();
    try {
      const keyboard = (window as any).veinjs;
      if (!keyboard || typeof keyboard.openosk !== 'function') throw new Error('keyboard unavailable');
      keyboard.openosk();
    } catch (error) {
      this.$Message.info('请使用系统屏幕键盘或外接键盘输入');
    }
  }

  /** 拍摄当前预览帧，只在内存中编码，不保存照片。 */
  private captureFace(): string {
    const camera = this.$refs.camera as HTMLVideoElement;
    if (!this.cameraReady || !camera || !camera.videoWidth || !camera.videoHeight)
      throw new Error('camera not ready');
    const canvas = document.createElement('canvas');
    canvas.width = camera.videoWidth;
    canvas.height = camera.videoHeight;
    const context = canvas.getContext('2d');
    if (!context) throw new Error('canvas unavailable');
    context.drawImage(camera, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL('image/jpeg', 0.9).split(',')[1];
  }

  /** 完成一种身份验证、初始化 WMS 会话，再继续已选择的样品业务。 */
  async submit() {
    if (this.submitting || !this.destination) return;
    if (this.registrationLogin && this.method !== 'password') return;
    this.errorMessage = '';
    if (this.method === 'password' && (!this.account.trim() || !this.password)) {
      this.errorMessage = '请输入账号和密码。';
      return;
    }
    let image = '';
    if (this.method === 'face') {
      try { image = this.captureFace(); }
      catch (error) { this.errorMessage = '摄像头画面尚未准备好，请稍候再试。'; return; }
    }
    const generation = ++this.generation;
    this.request = axios.CancelToken.source();
    this.submitting = true;
    try {
      const result = this.method === 'password'
        ? await passwordLogin(this.account.trim(), this.password, this.request.token)
        : await biometricLogin(this.method, image, this.request.token);
      if (generation !== this.generation) return;
      if (!result || !result.token) throw new Error('missing login token');
      Util.abp.auth.setToken(result.token);
      Util.abp.utils.deleteCookie(AppConsts.authorization.encrptedAuthTokenName, Util.abp.appPath);
      sessionStorage.setItem('rememberMe', '0');
      await this.$store.dispatch('session/init', { cancelToken: this.request.token });
      if (generation !== this.generation) return;
      const user = this.$store.state.session.user;
      if (!user || !user.id) throw new Error('missing session user');
      if (this.registrationLogin) {
        if (!isTerminalAdmin(user)) {
          this.clearSession();
          this.errorMessage = '当前账号不是管理员，请使用管理员账号登录。';
          return;
        }
        await checkRegistrationAccess(this.request.token);
        if (generation !== this.generation) return;
      }
      Cookies.set('facing', '0');
      Cookies.set('userId', String(user.id));
      Cookies.set('last_page_name', this.destination);
      this.completed = true;
      this.password = '';
      this.$router.replace({ name: this.destination });
    } catch (error) {
      if (generation === this.generation && !axios.isCancel(error)) {
        this.clearSession();
        this.errorMessage = this.registrationLogin ? registrationError(error) : terminalLoginError(error);
      }
    } finally {
      if (generation === this.generation) { this.submitting = false; this.request = null; }
    }
  }

  /** 清除终端身份，防止上一位操作人的令牌或身份 Cookie 被继续使用。 */
  private clearSession() {
    Util.abp.auth.clearToken();
    Util.abp.utils.deleteCookie(AppConsts.authorization.encrptedAuthTokenName, Util.abp.appPath);
    Cookies.set('facing', '0');
    Cookies.set('userId', '0');
    this.$store.state.session.user = null;
    Util.abp.session.userId = null;
  }

  /** 返回操作台，取消当前网络请求并清除尚未完成的登录状态。 */
  back() {
    this.cancelPending();
    this.$router.replace({ name: 'client' });
  }

  /** 页面销毁和手动返回共用资源清理，迟到响应不得触发业务跳转。 */
  private cancelPending() {
    this.generation++;
    if (this.request) this.request.cancel('terminal login cancelled');
    this.request = null;
    this.resetCredentials();
    this.closeCamera();
    if (!this.completed) this.clearSession();
  }

  /** 离开登录页面时关闭摄像头并取消尚未完成的登录请求。 */
  beforeDestroy() { this.cancelPending(); }
}
</script>

<style scoped lang="less">
@import './terminal-login.less';
</style>
