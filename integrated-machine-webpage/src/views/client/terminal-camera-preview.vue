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
    <div class="sample-login-camera">
      <video ref="video" autoplay muted playsinline @loadeddata="ready = true"></video>
      <span v-if="!ready">{{ opening ? '正在开启外接摄像头…' : '请开启外接摄像头' }}</span>
      <div class="sample-login-face-frame" aria-hidden="true"></div>
    </div>
    <button type="button" class="sample-login-submit" :disabled="disabled || opening" @click="open">{{ ready ? '重新开启摄像头' : '开启外接摄像头' }}</button>
    <p v-if="errorMessage" class="sample-login-error" role="alert">{{ errorMessage }}</p>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import { externalCameraDevices, selectExternalCamera, externalCameraConstraints, terminalCameraError } from '../../lib/terminal-camera';

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
      const devices = await navigator.mediaDevices.enumerateDevices();
      if (generation !== this.generation) return;
      this.devices = externalCameraDevices(devices);
      this.deviceId = selectExternalCamera(this.devices, this.deviceId);
      if (!this.devices.length) this.errorMessage = '未找到外接摄像头，请检查连接和摄像头权限后刷新。';
    } catch (error) {
      if (generation === this.generation) this.errorMessage = error.message || terminalCameraError(error);
    } finally {
      if (generation === this.generation) this.opening = false;
    }
  }

  /** 切换设备时先释放旧摄像头，再打开管理员选择的设备。 */
  changeDevice() { if (!this.disabled && !this.opening) this.open(); }

  /** 打开所选设备，权限响应延迟时仍确保流只属于当前组件。 */
  async open() {
    if (this.disabled || this.opening) return;
    if (!this.deviceId) { this.errorMessage = '请先选择外接摄像头，首次授权前名称可能不可见。'; return; }
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
      const devices = await navigator.mediaDevices.enumerateDevices();
      if (generation !== this.generation) return;
      this.devices = externalCameraDevices(devices);
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
