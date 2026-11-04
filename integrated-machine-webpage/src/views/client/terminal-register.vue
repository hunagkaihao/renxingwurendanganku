<template>
  <div class="sample-login-page">
    <header class="sample-login-header">
      <button type="button" class="sample-login-back" :disabled="busy" @click="back"><Icon type="ios-arrow-back" /> 退出注册</button>
      <span>人员注册 · 管理员操作</span>
    </header>
    <main class="sample-login-main">
      <section class="sample-login-card registration-card">
        <span class="sample-login-badge">人员与特征管理</span>
        <h1>人员注册</h1>
        <p class="sample-login-description">选择已有人员或新增账号，再录入人脸、指静脉</p>
        <p v-if="!authorized && !errorMessage" role="status">正在验证管理员权限…</p>
        <template v-if="authorized">
          <div class="registration-tabs" role="tablist" aria-label="人员来源">
            <button type="button" role="tab" :aria-selected="mode === 'existing'" :class="{ active: mode === 'existing' }" :disabled="busy" @click="mode = 'existing'">已有 WMS 人员</button>
            <button type="button" role="tab" :aria-selected="mode === 'new'" :class="{ active: mode === 'new' }" :disabled="busy" @click="showCreate">新增人员</button>
          </div>
          <div class="sample-login-panel">
            <form v-if="mode === 'new'" @submit.prevent="createUser">
              <label for="new-person-account">登录账号</label>
              <input id="new-person-account" v-model="newUser.userName" required maxlength="256" autocomplete="off" :disabled="busy">
              <label for="new-person-name">姓名</label>
              <input id="new-person-name" v-model="newUser.name" required maxlength="64" :disabled="busy">
              <label for="new-person-email">邮箱</label>
              <input id="new-person-email" v-model="newUser.email" type="email" required maxlength="256" :disabled="busy">
              <label for="new-person-password">初始密码</label>
              <input id="new-person-password" v-model="newUser.password" type="password" required autocomplete="new-password" :disabled="busy">
              <label for="new-person-confirm">确认初始密码</label>
              <input id="new-person-confirm" v-model="confirmPassword" type="password" required autocomplete="new-password" :disabled="busy">
              <fieldset class="registration-roles" :disabled="busy"><legend>分配角色（至少一个）</legend>
                <label v-for="role in roles" :key="role.id"><input v-model="newUser.roleNames" type="checkbox" :value="role.name"> {{ role.name }}</label>
                <p v-if="!roles.length">没有可选角色，请检查角色配置和管理员权限。</p>
              </fieldset>
              <button type="submit" class="sample-login-submit" :disabled="busy || !roles.length">{{ busy ? '正在处理…' : '创建账号并继续录入' }}</button>
            </form>
            <template v-else>
              <form class="registration-search" @submit.prevent="search(1)">
                <input v-model="filter" aria-label="搜索人员" placeholder="按账号或姓名搜索人员" :disabled="busy">
                <button type="submit" :disabled="busy">查询人员</button>
              </form>
              <label for="registration-person">选择 WMS 人员</label>
              <select id="registration-person" v-model="selectedUserId" :disabled="busy" @change="selectUser">
                <option value="" disabled>请选择需要录入的人员</option>
                <option v-for="user in users" :key="user.id" :value="user.id" :disabled="user.isActive === false">{{ user.userName }}{{ user.name ? ' · ' + user.name : '' }}{{ user.isActive === false ? '（已停用）' : '' }}</option>
              </select>
              <div class="registration-pagination">
                <button type="button" :disabled="busy || pageIndex <= 1" @click="search(pageIndex - 1)">上一页</button>
                <span>第 {{ pageIndex }} 页 · 共 {{ totalCount }} 人</span>
                <button type="button" :disabled="busy || pageIndex * 20 >= totalCount" @click="search(pageIndex + 1)">下一页</button>
              </div>
              <template v-if="selectedUser">
                <p class="registration-selected">正在为 <strong>{{ selectedUser.name || selectedUser.userName }}</strong>（{{ selectedUser.userName }}）录入</p>
                <div class="registration-bindings">
                  <strong>当前特征绑定</strong>
                  <p v-if="!bindings.length">尚未绑定特征</p>
                  <p v-for="binding in bindings" :key="binding.id">{{ binding.faceId ? '人脸 ' + binding.faceId : '指静脉 ' + binding.fingerId }} · {{ binding.isEnabled ? '已启用' : '未启用，需核查或继续录入' }}</p>
                  <button type="button" :disabled="busy" @click="refreshBindings">刷新绑定</button>
                </div>
                <div class="registration-tabs" role="tablist" aria-label="录入方式">
                  <button type="button" role="tab" :aria-selected="method === 'face'" :class="{ active: method === 'face' }" :disabled="busy" @click="changeMethod('face')">录入人脸</button>
                  <button type="button" role="tab" :aria-selected="method === 'vein'" :class="{ active: method === 'vein' }" :disabled="busy" @click="changeMethod('vein')">录入指静脉</button>
                </div>
                <p class="sample-login-description">{{ method === 'face' ? '人脸人员编号' : '指静脉模板编号' }}由系统自动生成，无需填写；录入失败后为同一人员重试会自动沿用预留编号。</p>
                <terminal-camera-preview v-if="method === 'face'" ref="camera" :disabled="busy" />
                <div v-else class="sample-login-device">
                  <div class="sample-login-finger"><img src="../../images/finger.png" alt="指静脉采集示意"></div>
                  <p>全程使用同一根手指，每次放回时保持相同位置和方向；采集成功后完全移开，等待下一次提示。</p>
                  <div v-if="veinProgress" class="registration-vein-progress" :class="'is-' + veinProgress.stage" role="status" aria-live="polite">
                    <p class="registration-vein-prompt">{{ veinProgress.message }}</p>
                    <p class="registration-vein-count">已采集 {{ veinProgress.completedSamples }} / 6 次</p>
                    <ol class="registration-vein-steps" aria-label="六次采集进度">
                      <li v-for="step in 6" :key="step" :class="{ completed: step <= veinProgress.completedSamples }" :aria-label="'第' + step + '次' + (step <= veinProgress.completedSamples ? '已采集' : '待采集')">{{ step <= veinProgress.completedSamples ? '✓' : step }}</li>
                    </ol>
                  </div>
                  <p v-else>点击开始后，请根据本页面的实时提示操作，共采集六次。</p>
                  <p v-if="progressWarning" class="registration-progress-warning" role="status">{{ progressWarning }}</p>
                </div>
                <button type="button" class="sample-login-submit" :disabled="busy" @click="enroll">{{ busy ? '正在录入，请稍候…' : (method === 'face' ? '拍照录入人脸并绑定账号' : '开始六次指静脉录入并绑定账号') }}</button>
              </template>
            </template>
          </div>
        </template>
        <p v-if="errorMessage" class="sample-login-error" role="alert">{{ errorMessage }}</p>
        <p v-if="successMessage" class="registration-success" role="status">{{ successMessage }}</p>
        <p class="sample-login-tip">录入成功后自动绑定所选账号。操作期间请等待结果，退出注册后将清除管理员会话。</p>
      </section>
    </main>
  </div>
</template>
<script lang="ts">
import { Component } from 'vue-property-decorator';
import axios, { CancelTokenSource } from 'axios';
import Cookies from 'js-cookie';
import AbpBase from '../../lib/abpbase';
import Util from '../../lib/util';
import AppConsts from '../../lib/appconst';
import TerminalCameraPreview from './terminal-camera-preview.vue';
import { RegistrationUser, RegistrationProgress, isTerminalAdmin, checkRegistrationAccess, searchRegistrationUsers, registrationRoles,
  createRegistrationUser, registrationBindings, enrollRegistrationUser, registrationError,
  createRegistrationOperationId, registrationProgress } from '../../lib/terminal-registration';

/** 管理员专用人员注册页，复用 WMS 账号和绑定，所有设备操作均经 WMS 授权后执行。 */
@Component({ components: { TerminalCameraPreview } })
export default class TerminalRegister extends AbpBase {
  /** 服务端已确认管理员权限后才显示人员及设备操作。 */
  authorized: boolean = false;
  /** 查询、创建或采集中禁用重复操作，避免覆盖人员选择或并发占用设备。 */
  busy: boolean = false;
  /** 已有人员选择或新账号创建模式。 */
  mode: string = 'existing';
  /** 当前查询关键词。 */
  filter: string = '';
  /** 当前人员列表页，从 1 开始。 */
  pageIndex: number = 1;
  /** 查询匹配的人员总数。 */
  totalCount: number = 0;
  /** 当前页已有账号。 */
  users: RegistrationUser[] = [];
  /** 当前选定账号的稳定 GUID。 */
  selectedUserId: string = '';
  /** 当前人员的绑定与未启用预留记录。 */
  bindings: any[] = [];
  /** 新人员可分配的现有角色。 */
  roles: any[] = [];
  /** 新账号资料只保存在当前页面，创建成功或离开时清除密码。 */
  newUser = { userName: '', name: '', email: '', password: '', roleNames: [] as string[] };
  /** 初始密码二次确认值。 */
  confirmPassword: string = '';
  /** 当前录入方式：face 或 vein。 */
  method: string = 'face';
  /** 最近一次操作失败原因。 */
  errorMessage: string = '';
  /** 最近一次创建或录入的确认结果。 */
  successMessage: string = '';
  /** 设备返回的本次指静脉进度，只在原录入请求成功后展示绑定完成。 */
  veinProgress: RegistrationProgress | null = null;
  /** 进度查询故障单独提示，不中断仍在执行的设备录入。 */
  progressWarning: string = '';
  /** 顺序轮询的下一次定时器，避免慢请求造成重叠查询。 */
  private progressTimer: ReturnType<typeof setTimeout> | null = null;
  /** 当前进度查询可单独取消，不能因此取消原录入请求。 */
  private progressRequest: CancelTokenSource | null = null;
  /** 只接收当前操作的提示，隔离迟到响应。 */
  private operationId: string = '';
  /** 页面离开时取消仍在等待的 HTTP 响应，不将迟到结果写回界面。 */
  private request: CancelTokenSource = axios.CancelToken.source();
  /** 组件是否已经离开，避免异步响应触发新操作。 */
  private disposed: boolean = false;

  /** 当前选定的人员，搜索翻页后必须重新选择，避免将特征录给隐藏的旧选择。 */
  get selectedUser(): RegistrationUser | undefined { return this.users.find(user => user.id === this.selectedUserId); }

  /** 先检查服务端权限，通过后才查询人员。 */
  async created() {
    if (!isTerminalAdmin(this.$store.state.session.user)) { this.requireLogin(); return; }
    try {
      await checkRegistrationAccess(this.request.token);
      if (this.disposed) return;
      this.authorized = true;
      await this.search(1);
    } catch (error) {
      if (!this.disposed && !axios.isCancel(error)) {
        this.errorMessage = registrationError(error);
        this.authorized = false;
      }
    }
  }

  /** 查询指定页人员，开始查询时清空旧目标，确保录入对象与界面一致。 */
  async search(page: number) {
    if (!this.authorized || this.busy) return;
    this.clearProgress();
    this.busy = true;
    this.errorMessage = '';
    this.selectedUserId = '';
    this.bindings = [];
    try {
      const result = await searchRegistrationUsers(this.filter.trim(), page, this.request.token);
      if (this.disposed) return;
      this.users = result.items || [];
      this.totalCount = result.totalCount;
      this.pageIndex = page;
    } catch (error) { this.reportError(error); }
    finally { this.busy = false; }
  }

  /** 选人后清空旧提示并重新查询绑定。 */
  async selectUser() {
    this.clearProgress();
    this.successMessage = '';
    this.bindings = [];
    await this.refreshBindings();
  }

  /** 查询所选账号的绑定，失败时保留明确错误，不影响已经完成的设备结果。 */
  async refreshBindings() {
    if (!this.authorized || this.busy || !this.selectedUserId) return;
    this.busy = true;
    this.errorMessage = '';
    try {
      const bindings = await registrationBindings(this.selectedUserId, this.request.token);
      if (!this.disposed) this.bindings = bindings;
    } catch (error) { this.reportError(error); }
    finally { this.busy = false; }
  }

  /** 新增账号前加载可分配角色，不假定某个业务角色必然存在。 */
  async showCreate() {
    if (this.busy) return;
    this.clearProgress();
    this.mode = 'new';
    this.errorMessage = '';
    this.successMessage = '';
    this.busy = true;
    try {
      const roles = await registrationRoles(this.request.token);
      if (!this.disposed) this.roles = roles;
    } catch (error) { this.reportError(error); }
    finally { this.busy = false; }
  }

  /** 创建成功后直接选中新账号进入录入；设备录入失败不会重复创建账号。 */
  async createUser() {
    if (!this.authorized || this.busy) return;
    this.errorMessage = '';
    if (!this.newUser.userName.trim() || !this.newUser.name.trim() || !this.newUser.email.trim() || !this.newUser.password) {
      this.errorMessage = '请完整填写账号、姓名、邮箱和初始密码。'; return;
    }
    if (this.newUser.password !== this.confirmPassword) { this.errorMessage = '两次输入的初始密码不一致。'; return; }
    if (!this.newUser.roleNames.length) { this.errorMessage = '请至少分配一个角色。'; return; }
    this.busy = true;
    this.successMessage = '';
    try {
      const user = await createRegistrationUser({ ...this.newUser, userName: this.newUser.userName.trim(),
        name: this.newUser.name.trim(), email: this.newUser.email.trim(), isActive: true }, this.request.token);
      if (this.disposed) return;
      this.users = [user];
      this.selectedUserId = user.id;
      this.bindings = [];
      this.mode = 'existing';
      this.pageIndex = 1;
      this.totalCount = 1;
      this.newUser = { userName: '', name: '', email: '', password: '', roleNames: [] };
      this.confirmPassword = '';
      this.successMessage = 'WMS 账号创建成功，请继续录入人脸或指静脉。';
    } catch (error) { this.reportError(error); }
    finally { this.busy = false; }
  }

  /** 切换录入方式时清空上一次操作提示。 */
  changeMethod(method: string) {
    if (this.busy || this.method === method) return;
    this.clearProgress();
    this.method = method;
    this.errorMessage = '';
    this.successMessage = '';
  }

  /** 以当前所选账号为唯一录入目标，成功响应后更新绑定，失败或超时不自动重试。 */
  async enroll() {
    if (!this.authorized || this.busy || !this.selectedUser) return;
    this.clearProgress();
    this.errorMessage = '';
    this.successMessage = '';
    try {
      const image = this.method === 'face' ? (this.$refs.camera as TerminalCameraPreview).capture() : '';
      const operationId = this.method === 'vein' ? createRegistrationOperationId() : undefined;
      this.busy = true;
      const enrollment = enrollRegistrationUser(this.selectedUserId, this.method, undefined, image, this.request.token, operationId);
      if (operationId) this.startProgress(this.selectedUserId, operationId);
      const binding = await enrollment;
      if (this.disposed) return;
      this.bindings = this.bindings.filter(item => item.id !== binding.id).concat([binding]);
      this.successMessage = (this.method === 'face' ? '人脸' : '指静脉') + '录入成功，已绑定当前账号，可用于登录。';
      if (this.veinProgress) this.veinProgress = { ...this.veinProgress, stage: 'completed', message: this.successMessage,
        completedSamples: 6, isComplete: true, success: true };
      this.progressWarning = '';
    } catch (error) {
      this.reportError(error);
      if (!this.disposed && this.veinProgress) this.veinProgress = { ...this.veinProgress, stage: 'failed',
        message: '本次录入未完成，请查看下方原因。', isComplete: true, success: false };
    }
    finally { this.stopProgress(); this.busy = false; }
  }

  /** 原录入请求发出后开始查询，不通过计时推测采集成功。 */
  private startProgress(userId: string, operationId: string) {
    this.operationId = operationId;
    this.veinProgress = { operationId, stage: 'preparing', message: '正在连接设备，请准备使用同一根手指完成六次录入。',
      completedSamples: 0, totalSamples: 6, isComplete: false };
    this.progressTimer = setTimeout(() => this.pollProgress(userId, operationId), 0);
  }

  /** 顺序读取设备当前指令；查询故障保留原录入请求，恢复连接后继续接收提示。 */
  private async pollProgress(userId: string, operationId: string) {
    if (this.disposed || this.operationId !== operationId || !this.busy) return;
    this.progressTimer = null;
    const request = axios.CancelToken.source();
    this.progressRequest = request;
    try {
      const progress = await registrationProgress(userId, operationId, request.token);
      if (this.disposed || this.operationId !== operationId) return;
      this.veinProgress = progress;
      this.progressWarning = '';
    } catch (error) {
      if (!this.disposed && this.operationId === operationId && !axios.isCancel(error)) {
        this.progressWarning = registrationError(error);
        if (this.veinProgress && !this.veinProgress.isComplete) this.veinProgress = { ...this.veinProgress,
          stage: 'unavailable', message: '实时提示暂不可用，请查看设备终端提示，等待本次录入结果。' };
      }
    } finally {
      if (this.progressRequest === request) this.progressRequest = null;
      if (!this.disposed && this.operationId === operationId && this.busy && this.veinProgress && !this.veinProgress.isComplete)
        this.progressTimer = setTimeout(() => this.pollProgress(userId, operationId), 400);
    }
  }

  /** 原请求结束或离开页面时停止查询，迟到响应由操作标识检查丢弃。 */
  private stopProgress() {
    this.operationId = '';
    if (this.progressTimer !== null) clearTimeout(this.progressTimer);
    this.progressTimer = null;
    if (this.progressRequest) this.progressRequest.cancel('registration progress stopped');
    this.progressRequest = null;
  }

  /** 更换人员、方式或开始新操作时清除上一次进度，避免提示指向旧目标。 */
  private clearProgress() {
    this.stopProgress();
    this.veinProgress = null;
    this.progressWarning = '';
  }

  /** 只处理仍属于本页面的失败；权限失效时立即隐藏操作区域。 */
  private reportError(error: any) {
    if (this.disposed || axios.isCancel(error)) return;
    this.errorMessage = registrationError(error);
    if (error.response && (error.response.status === 401 || error.response.status === 403)) this.authorized = false;
  }

  /** 当前会话不是管理员时转到本轮账号密码验证。 */
  private requireLogin() { this.$router.replace({ name: 'terminal-login', query: { next: 'userregister' } }); }

  /** 主动退出人员注册，清理工作由组件销毁钩子统一执行。 */
  back() { if (!this.busy) this.$router.replace({ name: 'client' }); }

  /** 离开页面清除管理员身份、敏感表单和迟到请求，防止下一位使用者继承管理会话。 */
  beforeDestroy() {
    this.disposed = true;
    this.stopProgress();
    this.request.cancel('registration page closed');
    this.newUser.password = '';
    this.confirmPassword = '';
    Util.abp.auth.clearToken();
    Util.abp.utils.deleteCookie(AppConsts.authorization.encrptedAuthTokenName, Util.abp.appPath);
    this.$store.state.session.user = null;
    Util.abp.session.userId = null;
    Cookies.set('facing', '0');
    Cookies.set('userId', '0');
  }
}
</script>
<style scoped lang="less">
@import './terminal-login.less';
@import './terminal-register.less';
</style>
