<template>
  <div class="registration-camera">
    <label for="registration-camera-device">外接摄像头</label>
    <div class="sample-login-camera-controls">
      <select id="registration-camera-device" v-model="deviceId" :disabled="disabled || opening" @change="changeDevice">
        <option value="" disabled>请选择外接摄像头</option>
        <option v-for="(device, index) in devices" :key="device.deviceId" :value="device.deviceId">
          {{ (device.label || '摄像头（授权后显示名称）') + ' · ' + (index + 1) }}
        </option>
      </select>
      <button type="button" :disabled="disabled || opening" @click="refresh">刷新设备</button>
    </div>
    <div v-if="needsAuthorization" class="sample-login-camera-authorization">
      <p>读取完整设备列表可能短暂打开浏览器默认摄像头，列出设备后立即关闭，不用于预览或拍照。</p>
      <button type="button" :disabled="disabled || opening" @click="authorize">{{ permissionState === 'granted' ? '初始化并刷新设备' : '授权并刷新设备' }}</button>
      <p>如不希望打开默认摄像头，请在浏览器地址栏的站点设置中允许摄像头，并在浏览器摄像头设置中选择外接 RGB Camera，再点击刷新设备。</p>
    </div>
    <p v-if="needsAuthorization && deviceNotice" class="sample-login-camera-notice" role="status">{{ deviceNotice }}</p>
    <div class="sample-login-camera">
      <video ref="video" autoplay muted playsinline @loadeddata="ready = true"></video>
      <span v-if="!ready">{{ opening ? '正在开启外接摄像头…' : '请开启外接摄像头' }}</span>
      <svg class="sample-login-face-frame" viewBox="0 0 180 240" aria-hidden="true" focusable="false">
        <path d="M90 8 C48 8 20 31 18 74 C15 122 29 175 54 211 C65 227 77 234 90 234 C103 234 115 227 126 211 C151 175 165 122 162 74 C160 31 132 8 90 8 Z" />
      </svg>
    </div>
    <button type="button" class="sample-login-submit" :disabled="disabled || opening" @click="open">{{ ready ? '重新开启摄像头' : '开启外接摄像头' }}</button>
    <p v-if="errorMessage" class="sample-login-error" role="alert">{{ errorMessage }}</p>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import { CameraPermissionState, readCameraPermissionState, inspectExternalCameras, authorizeCameraDevices, selectExternalCamera, externalCameraConstraints, terminalCameraError } from '../../lib/terminal-camera';

/** 注册页的外接摄像头预览，仅在内存中采集当前照片，销毁时释放所有设备轨道。 */
@Component
export default class TerminalCameraPreview extends Vue {
  /** 录入过程中禁止切换或重新打开设备。 */
  @Prop({ type: Boolean, default: false }) disabled: boolean;
  /** 浏览器可见的外接候选设备。 */
  devices: MediaDeviceInfo[] = [];
  /** 用户选择的精确设备 ID，不允许回退到默认摄像头。 */
  deviceId: string = '';
  /** 当前预览已输出可采集的帧。 */
  ready: boolean = false;
  /** 正在申请设备或加载设备列表。 */
  opening: boolean = false;
  /** 摄像头操作失败原因。 */
  errorMessage: string = '';
  /** 设备信息被浏览器隐藏时显示独立授权入口，正式开启仍要求精确选择设备。 */
  needsAuthorization: boolean = false;
  /** 设备列表的授权说明或双目通道选择提示。 */
  deviceNotice: string = '';
  /** 只读查询得到的当前站点权限，不代表设备一定可读取。 */
  permissionState: CameraPermissionState = 'unknown';
  /** 本组件独占的视频流。 */
  private stream: MediaStream | null = null;
  /** 异步操作代次，拦截组件销毁后的设备响应。 */
  private generation: number = 0;

  /** 进入人脸录入时只列出设备，等待管理员选择或开启。 */
  mounted() { this.refresh(); }

  /** 更新设备列表并保留有效选择；多个 RGB 候选时要求明确选择。 */
  async refresh() {
    if (this.disabled || this.opening) return;
    const generation = ++this.generation;
    this.close();
    this.opening = true;
    this.errorMessage = '';
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices)
        throw new Error('请通过 localhost、127.0.0.1 或 HTTPS 访问并允许摄像头权限。');
      const [devices, permission] = await Promise.all([
        navigator.mediaDevices.enumerateDevices(), readCameraPermissionState(navigator.permissions)
      ]);
      if (generation !== this.generation) return;
      this.permissionState = permission;
      this.updateDevices(devices);
    } catch (error) {
      if (generation === this.generation) this.errorMessage = error.message || terminalCameraError(error);
    } finally {
      if (generation === this.generation) this.opening = false;
    }
  }

  /** 显式申请浏览器权限后重新列举设备；临时默认流不进入预览，也不自动开启候选设备。 */
  async authorize() {
    if (this.disabled || this.opening) return;
    const generation = ++this.generation;
    this.close();
    this.opening = true;
    this.errorMessage = '';
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia)
        throw new Error('请通过 localhost、127.0.0.1 或 HTTPS 访问并允许摄像头权限。');
      const devices = await authorizeCameraDevices(navigator.mediaDevices,
        () => generation === this.generation, stream => { this.stream = stream; });
      const permission = await readCameraPermissionState(navigator.permissions);
      if (generation !== this.generation) return;
      this.permissionState = permission;
      this.updateDevices(devices);
    } catch (error) {
      if (generation === this.generation)
        this.errorMessage = error.name === 'Error' ? error.message : terminalCameraError(error);
    } finally {
      if (generation === this.generation) { this.close(); this.opening = false; }
    }
  }

  /** 按原始列表分析权限；已有视频流时禁止自动换选其他设备，保留双目各通道的准确对应。 */
  private updateDevices(devices: MediaDeviceInfo[], allowAutoSelect: boolean = true) {
    const discovery = inspectExternalCameras(devices, this.permissionState);
    this.devices = discovery.devices;
    this.needsAuthorization = discovery.needsAuthorization;
    this.deviceNotice = discovery.message;
    if (!this.devices.some(device => device.deviceId === this.deviceId))
      this.deviceId = allowAutoSelect ? selectExternalCamera(this.devices, '') : '';
  }

  /** 切换设备时先释放旧摄像头，再打开管理员选择的设备。 */
  changeDevice() { if (!this.disabled && !this.opening) this.open(); }

  /** 打开所选设备，权限响应延迟时仍确保流只属于当前组件。 */
  async open() {
    if (this.disabled || this.opening) return;
    if (!this.deviceId) {
      this.errorMessage = this.needsAuthorization ? '请先点击上方的设备初始化或授权按钮刷新列表，再选择外接 RGB 摄像头。' : '请先选择外接摄像头。';
      return;
    }
    const generation = ++this.generation;
    this.close();
    this.opening = true;
    this.errorMessage = '';
    try {
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia)
        throw new Error('当前浏览器无法采集摄像头，请通过本机地址或 HTTPS 访问。');
      const stream = await navigator.mediaDevices.getUserMedia(externalCameraConstraints(this.deviceId));
      if (generation !== this.generation) { stream.getTracks().forEach(track => track.stop()); return; }
      this.stream = stream;
      const [devices, permission] = await Promise.all([
        navigator.mediaDevices.enumerateDevices(), readCameraPermissionState(navigator.permissions)
      ]);
      if (generation !== this.generation) return;
      this.permissionState = permission;
      this.updateDevices(devices, false);
      if (!this.devices.some(device => device.deviceId === this.deviceId)) {
        this.deviceId = '';
        throw new Error('该设备不是可用的外接摄像头，请重新选择。');
      }
      const video = this.$refs.video as HTMLVideoElement;
      video.srcObject = stream;
      await video.play();
    } catch (error) {
      if (generation === this.generation) {
        this.close();
        if (error.name === 'NotAllowedError' || error.name === 'PermissionDeniedError') this.needsAuthorization = true;
        this.errorMessage = error.name === 'Error' ? error.message : terminalCameraError(error);
      }
    } finally {
      if (generation === this.generation) this.opening = false;
    }
  }

  /** 拍摄当前帧并返回 JPEG Base64，不在磁盘或浏览器存储中保留照片。 */
  capture(): string {
    const video = this.$refs.video as HTMLVideoElement;
    if (!this.ready || !video || !video.videoWidth || !video.videoHeight) throw new Error('请先开启外接摄像头，等待画面就绪。');
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const context = canvas.getContext('2d');
    if (!context) throw new Error('当前浏览器无法拍摄照片。');
    context.drawImage(video, 0, 0);
    return canvas.toDataURL('image/jpeg', 0.9).split(',')[1];
  }

  /** 释放组件持有的设备流并清空预览状态。 */
  close() {
    if (this.stream) this.stream.getTracks().forEach(track => track.stop());
    this.stream = null;
    this.ready = false;
    const video = this.$refs.video as HTMLVideoElement;
    if (video) video.srcObject = null;
  }

  /** 离开录入页或切换录入方式时终止迟到响应并释放设备。 */
  beforeDestroy() { this.generation++; this.close(); }
}
</script>
<style scoped lang="less">
@import './terminal-login.less';
</style>
