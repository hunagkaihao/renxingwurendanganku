<script setup>
import { nextTick, onBeforeUnmount, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { interpretResult, request, validateImage, validateUserId } from './api.js'

// 测试数据仅保存在当前页面内存，不持久保存图片、编号或响应。
const tab = ref('face')
const apiBase = ref('/api')
const userId = ref('123456')
const fingerId = ref('123')
const photo = ref('')
const photoName = ref('尚未选择照片')
const busy = ref('')
const result = ref(null)
const rawResult = ref('')
const video = ref(null)
const cameraOpen = ref(false)
const cameraStarting = ref(false)
let stream = null
let disposed = false

/** 选择照片时先校验大小与格式，再读取数据 URL，失败不保留旧照片。 */
async function selectPhoto(event) {
  const file = event.target.files?.[0]
  if (!file) return
  photo.value = ''
  photoName.value = '尚未选择照片'
  try {
    validateImage(file)
    const data = await new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.onload = () => resolve(reader.result)
      reader.onerror = () => reject(new Error('读取照片失败。'))
      reader.readAsDataURL(file)
    })
    // 确认浏览器可解码，具体人脸质量仍由后端判断。
    await new Promise((resolve, reject) => {
      const image = new Image()
      image.onload = resolve
      image.onerror = () => reject(new Error('图片内容无法解码，请重新选择。'))
      image.src = data
    })
    photo.value = data
    photoName.value = `${file.name} · ${(file.size / 1_000_000).toFixed(2)} MB`
  } catch (error) { ElMessage.error(error.message) }
  finally { event.target.value = '' }
}

/** 停止本页面拥有的摄像头轨道，释放设备占用。 */
function stopCamera() {
  stream?.getTracks().forEach((track) => track.stop())
  stream = null
  if (video.value) video.value.srcObject = null
  cameraOpen.value = false
}

/** 仅用户点击后申请摄像头，处理拒绝权限及设备不可用。 */
async function startCamera() {
  cameraStarting.value = true
  try {
    if (!navigator.mediaDevices?.getUserMedia) throw new Error('摄像头需要 localhost 或 HTTPS 安全环境。')
    const acquired = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user', width: { ideal: 1280 }, height: { ideal: 720 } }, audio: false })
    if (disposed) { acquired.getTracks().forEach((track) => track.stop()); return }
    stream = acquired
    cameraOpen.value = true
    await nextTick()
    video.value.srcObject = stream
    await video.value.play()
  } catch (error) {
    stopCamera()
    ElMessage.error(error.name === 'NotAllowedError' ? '摄像头权限被拒绝，请允许权限或上传照片。' : error.message)
  } finally { cameraStarting.value = false }
}

/** 将当前视频帧转换为 JPEG；不镜像提交图片。 */
function capturePhoto() {
  if (!video.value?.videoWidth) { ElMessage.warning('摄像头画面尚未就绪。'); return }
  const canvas = document.createElement('canvas')
  canvas.width = video.value.videoWidth
  canvas.height = video.value.videoHeight
  canvas.getContext('2d').drawImage(video.value, 0, 0)
  const image = canvas.toDataURL('image/jpeg', 0.9)
  if (image.length > 5_400_000) { ElMessage.error('拍摄图片过大，请上传不超过 4 MB 的照片。'); return }
  photo.value = image
  photoName.value = `摄像头照片 · ${canvas.width} × ${canvas.height}`
  stopCamera()
}

/** 删除操作由用户确认，取消时不调用后端。 */
async function confirmDelete(message) {
  try { await ElMessageBox.confirm(message, '确认删除', { type: 'warning', confirmButtonText: '确认删除', cancelButtonText: '取消' }); return true }
  catch (reason) { if (reason === 'cancel' || reason === 'close') return false; throw reason }
}

/** 串行调用设备/API，展示业务状态和原始响应。 */
async function run(action) {
  if (busy.value) return
  busy.value = action
  try {
    const base = apiBase.value.trim().replace(/\/$/, '')
    if (base !== '/api') {
      const url = new URL(base)
      if (!['http:', 'https:'].includes(url.protocol) || url.search || url.hash || url.username || url.password) throw new Error('API 地址须为 HTTP(S) 地址，且不含查询参数、片段或凭据。')
    }
    let path, method = 'POST', body
    if (action.startsWith('face')) {
      if (action !== 'faceDelete' && !photo.value) throw new Error('请先上传照片或拍照。')
      if (action === 'faceMatch') { path = '/Face/FaceMatch'; body = { ImageData: photo.value } }
      else {
        const id = validateUserId(userId.value)
        if (action === 'faceDelete' && !await confirmDelete(`删除用户 ${id} 的全部人脸样本？`)) return
        path = action === 'faceAdd' ? '/Face/FaceAdd' : '/Face/FaceDelete'
        body = { UserId: id, ...(action === 'faceAdd' ? { ImageData: photo.value } : {}) }
      }
    } else if (action === 'veinRegister') {
      const id = validateUserId(fingerId.value)
      const user = validateUserId(userId.value)
      path = `/Vein/VeinRegisterResult?${new URLSearchParams({ userId: user, fingerId: id })}`
      method = 'POST'
    } else if (action === 'veinMatch') { path = '/Vein/VeinMatchResult'; method = 'GET' }
    else if (action === 'veinDelete') {
      const id = Number(validateUserId(fingerId.value))
      if (!await confirmDelete(`删除设备模板 ${id} 并请求删除业务后台对应记录？`)) return
      path = '/Vein/VeinDelete'; body = { VeinId: id }
    } else { path = '/swagger/v1/swagger.json'; method = 'GET' }
    const data = await request(base, path, method, body)
    if (action === 'connect') {
      result.value = data?.openapi && data?.paths?.['/Face/FaceMatch'] && data?.paths?.['/Vein/VeinMatch']
        ? { type: 'success', message: '已连接 ClientApi。' }
        : { type: 'error', message: '已收到响应，但不是预期的 ClientApi OpenAPI 接口。' }
      rawResult.value = JSON.stringify({ openapi: data?.openapi, info: data?.info }, null, 2)
    } else { result.value = interpretResult(action, data); rawResult.value = data === null ? '（无响应正文）' : JSON.stringify(data, null, 2) }
  } catch (error) {
    result.value = { type: 'error', message: error.message }
    rawResult.value = ''
  } finally {
    busy.value = ''
  }
}

onBeforeUnmount(() => { disposed = true; stopCamera() })
</script>

<template>
  <main class="shell">
    <h1>生物识别测试</h1>

    <section class="connection" aria-label="API 连接">
      <div class="connection-row"><label class="connection-title" for="api-address">API 地址</label>
        <el-input id="api-address" v-model="apiBase" :disabled="!!busy" placeholder="/api 或 http://localhost:5123" />
        <el-button :loading="busy === 'connect'" :disabled="!!busy" @click="run('connect')">检查连接</el-button>
      </div>
    </section>

    <div class="workspace">
      <section class="controls">
        <el-tabs v-model="tab" @tab-change="stopCamera">
          <el-tab-pane label="人脸识别" name="face">
            <div class="photo-stage">
              <video v-if="cameraOpen" ref="video" autoplay muted playsinline aria-label="摄像头实时画面"></video>
              <img v-else-if="photo" :src="photo" alt="待录入或识别的人脸照片" />
              <div v-else class="photo-empty"><strong>未选择照片</strong><span>JPEG / PNG / BMP · 最大 4 MB</span></div>
            </div>
            <div class="photo-toolbar">
              <label class="upload-button" :class="{ disabled: !!busy }">上传照片<input type="file" accept="image/jpeg,image/png,image/bmp" :disabled="!!busy" @change="selectPhoto" /></label>
              <el-button v-if="!cameraOpen" :loading="cameraStarting" :disabled="!!busy || cameraStarting" @click="startCamera">开启摄像头</el-button>
              <template v-else><el-button type="primary" :disabled="!!busy" @click="capturePhoto">拍摄照片</el-button><el-button @click="stopCamera">关闭摄像头</el-button></template>
            </div>
            <div class="photo-caption">{{ photoName }}</div>
            <el-form label-position="top"><el-form-item label="业务用户 ID"><el-input v-model="userId" :disabled="!!busy" placeholder="1～2147483647" /></el-form-item></el-form>
            <div class="action-row"><el-button type="primary" :disabled="!!busy || !photo" :loading="busy === 'faceAdd'" @click="run('faceAdd')">录入人脸</el-button><el-button :disabled="!!busy || !photo" :loading="busy === 'faceMatch'" @click="run('faceMatch')">识别人脸</el-button><el-button type="danger" plain :disabled="!!busy" @click="run('faceDelete')">删除样本</el-button></div>
          </el-tab-pane>

          <el-tab-pane label="指静脉识别" name="vein">
            <el-form label-position="top">
              <el-form-item label="设备模板编号"><el-input v-model="fingerId" :disabled="!!busy" placeholder="1～2147483647" /></el-form-item>
              <el-form-item label="业务用户 ID"><el-input v-model="userId" :disabled="!!busy" placeholder="1～2147483647" /></el-form-item>
            </el-form>
            <div class="action-row"><el-button type="primary" :disabled="!!busy" :loading="busy === 'veinRegister'" @click="run('veinRegister')">开始六次注册</el-button><el-button :disabled="!!busy" :loading="busy === 'veinMatch'" @click="run('veinMatch')">识别手指</el-button><el-button type="danger" plain :disabled="!!busy" @click="run('veinDelete')">删除模板</el-button></div>
          </el-tab-pane>
        </el-tabs>
      </section>

      <section class="results" aria-label="操作结果">
          <h2>操作结果</h2>
          <el-alert v-if="busy" :title="busy === 'veinRegister' ? '正在采集：按提示放入、移开同一手指六次…' : '正在请求…'" type="info" :closable="false" show-icon />
          <el-alert v-else-if="result" :title="result.message" :type="result.type" :closable="false" show-icon />
          <div v-else class="result-empty">暂无结果</div>
          <pre v-if="rawResult" class="response-code">{{ rawResult }}</pre>
      </section>
    </div>
  </main>
</template>
