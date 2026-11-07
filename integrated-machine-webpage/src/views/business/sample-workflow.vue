<template>
  <div class="sample-retention-page sample-workflow">
    <header class="workflow-header">
      <h1>{{ operation }}</h1>
      <div><span>{{ userName }}</span><button type="button" @click="returnHome">返回首页</button></div>
    </header>
    <main class="workflow-content">
      <p v-if="wmsOnly" class="workflow-info">当前为仅查询模式，可查看任务和库存；设备业务尚未接入。</p>
      <section class="sample-progress" :aria-label="operation + '进度'">
        <header><h2>{{ operation }}进度</h2><span data-testid="task-status" role="status">{{ progressSummary }}</span></header>
        <div class="sample-progress-scroll">
          <ol class="sample-progress-list">
            <li v-for="(step, index) in steps" :key="step.title"
              :class="{ 'is-complete': currentStep >= 0 && index < currentStep, 'is-current': index === currentStep }"
              :aria-current="index === currentStep ? 'step' : undefined">
              <span class="sample-progress-icon"><Icon :type="currentStep > index ? 'md-checkmark' : step.icon" size="25" /></span>
              <strong>{{ step.title }}</strong><small>{{ stepCaption(index) }}</small>
            </li>
          </ol>
        </div>
      </section>

      <section class="workflow-panel">
        <h2>{{ direction === 'in' ? '扫描已预约样品' : '选择取样样品' }}</h2>
        <form class="workflow-form" @submit.prevent="submit">
          <label>完整物料条码
            <input id="sample-barcode" v-model="barcode" autocomplete="off" :disabled="submitting || wmsOnly"
              :placeholder="direction === 'in' ? '请扫描或输入预约的完整条码' : '扫描条码，或在下方选择在库样品'" />
          </label>
          <label v-if="direction === 'out'">库位（选填）
            <input id="sample-cell" v-model="cellCode" autocomplete="off" :disabled="submitting || wmsOnly" placeholder="例如 01-002-01" />
          </label>
          <button type="submit" data-testid="submit-sample" :disabled="submitting || wmsOnly || !tasksFresh">
            {{ submitting ? '正在提交…' : (direction === 'in' ? '确认留样' : '下发取样任务') }}
          </button>
        </form>
        <p class="workflow-hint">{{ direction === 'in' ? '请先完成入库预约，再扫描实物条码。' : '物料条码与库位至少填写一个；同时填写时必须对应同一实物。' }}</p>
        <p v-if="actionError" class="workflow-error" role="alert" data-testid="action-error">{{ actionError }}</p>
        <p v-if="notice" class="workflow-notice" role="status">{{ notice }}</p>
      </section>

      <section class="workflow-panel" aria-label="任务清单">
        <header class="workflow-section-title"><h2>{{ operation }}任务</h2><button type="button" :disabled="loading" @click="refresh">{{ loading ? '刷新中…' : '刷新任务' }}</button></header>
        <p v-if="readError" class="workflow-error" role="alert">{{ readError }} 当前显示上次查询结果，设备操作暂不可用。</p>
        <div v-if="tasks.length" class="workflow-table-scroll">
          <table><thead><tr><th>任务号</th><th>实物条码</th><th>状态</th><th>起点</th><th>终点</th><th>查看</th></tr></thead>
            <tbody><tr v-for="task in taskRows" :key="task.id" :class="{ 'workflow-selected': task.id === selectedTaskId }">
              <td>{{ task.id }}</td><td>{{ task.materialBarcode }}</td><td>{{ taskLabel(task) }}</td><td>{{ task.startCellCode || '待分配' }}</td><td>{{ task.endCellCode || '待分配' }}</td>
              <td><button type="button" :data-task-id="task.id" @click="selectTask(task)">查看进度</button></td>
            </tr></tbody>
          </table>
        </div>
        <p v-else class="workflow-hint">{{ loading ? '正在读取任务…' : '暂无' + operation + '任务' }}</p>
        <div v-if="selectedTask" class="workflow-task-actions">
          <span>当前任务：{{ selectedTask.id }} · {{ selectedTask.materialBarcode }}</span>
          <button v-if="direction === 'in' && selectedTask.taskStatusString === 'WaitingExecute'" type="button"
            :disabled="wmsOnly || !tasksFresh || submitting" @click="dispatchReservation">下发此预约</button>
          <button v-if="canOpenDoor" type="button" data-testid="open-task-door" :disabled="doorPending || submitting"
            @click="openDoor">{{ doorPending ? '正在发送…' : '请求打开当前任务柜门' }}</button>
        </div>
      </section>

      <section v-if="direction === 'out'" class="workflow-panel" aria-label="在库样品">
        <header class="workflow-section-title"><h2>在库样品</h2><span>共 {{ inventoryTotal }} 条</span></header>
        <form class="workflow-form" @submit.prevent="searchInventory">
          <label>按库位筛选<input v-model="inventoryFilter" placeholder="输入库位编码或名称" /></label>
          <button type="submit" :disabled="inventoryLoading">查询库存</button>
        </form>
        <p v-if="inventoryError" class="workflow-error" role="alert">{{ inventoryError }}</p>
        <div class="workflow-table-scroll"><table><thead><tr><th>库位</th><th>实物条码</th><th>基础物料码</th><th>选择</th></tr></thead>
          <tbody><tr v-for="cell in inventory" :key="cell.id"><td>{{ cell.cellCode }}</td><td>{{ cell.materialBarcode }}</td><td>{{ cell.materialCode || '—' }}</td>
            <td><button type="button" :data-inventory-id="cell.id" :disabled="wmsOnly || submitting || !!inventoryError || !cell.materialBarcode" @click="chooseInventory(cell)">选择取样</button></td>
          </tr></tbody></table></div>
        <p v-if="!inventory.length && !inventoryLoading" class="workflow-hint">暂无符合条件的在库样品。</p>
        <div class="workflow-pagination"><button type="button" :disabled="inventoryPage <= 1 || inventoryLoading" @click="changeInventoryPage(-1)">上一页</button>
          <span>第 {{ inventoryPage }} 页</span><button type="button" :disabled="inventoryPage * 10 >= inventoryTotal || inventoryLoading" @click="changeInventoryPage(1)">下一页</button></div>
      </section>

      <sample-cabinets v-if="wmsOnly" :operation="operation" />
      <section v-else class="workflow-panel" aria-label="实际柜门任务">
        <header class="workflow-section-title"><h2>柜门任务</h2><span>已配置 {{ stations.length }} 个柜门</span></header>
        <p class="workflow-hint">选择柜门可查看关联任务，开门操作使用该任务的实际编号。</p>
        <p v-if="stationError" class="workflow-error" role="alert">{{ stationError }}</p>
        <div class="workflow-stations"><button v-for="station in stations" :key="station.id" type="button"
          :disabled="!taskForStation(station)" @click="selectTask(taskForStation(station))">
          <strong>{{ station.cellName || station.cellCode }}</strong><span>{{ station.cellCode }}</span>
          <small>{{ taskForStation(station) ? taskLabel(taskForStation(station)) : (station.runStatus === 'Enable' ? '当前无任务' : '暂不可用') }}</small>
        </button></div>
      </section>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Prop } from 'vue-property-decorator';
import axios, { CancelTokenSource } from 'axios';
import Cookies from 'js-cookie';
import AbpBase from '../../lib/abpbase';
import Util from '../../lib/util';
import AppConsts from '../../lib/appconst';
import SampleCabinets from './stockin/sample-cabinets.vue';
import { SampleDirection, SampleTask, SampleCell, sampleBarcode, sampleTaskMatches, sampleTaskEnded,
  sampleTaskStep, sampleTaskLabel, readSampleTasks, readSampleTaskHistory, readSampleStations, readSampleInventory,
  dispatchSampleIn, dispatchSampleOut, authorizeSampleDoor, sampleWorkflowError } from '../../lib/sample-workflow';

/** 四个终端路由共用真实 WMS 流程，避免旧接口、柜门编号及进度逻辑分叉。 */
@Component({ components: { SampleCabinets } })
export default class SampleWorkflow extends AbpBase {
  /** 当前路由对应留样或取样。 */
  @Prop({ type: String, required: true }) direction!: SampleDirection;
  /** 沿用仅 WMS 模式的设备隔离，查询仍使用当前接口。 */
  wmsOnly = AppConsts.wmsOnly;
  /** 原样保存扫码输入，仅提交时去除前后空白。 */
  barcode = '';
  /** 可选出库库位编码，与条码同时填写时由服务端核对。 */
  cellCode = '';
  /** 当前方向的任务，包含近期已完成任务供核对。 */
  tasks: SampleTask[] = [];
  /** 选中 WMS 任务主键，不与设备编号混用。 */
  selectedTaskId: number | null = null;
  /** 任务清单最近一次读取是否成功，失败时保留上次数据但禁止设备操作。 */
  tasksFresh = false;
  /** 当前已成功提交、等待任务清单确认的完整条码。 */
  awaitingBarcode = '';
  /** 写请求互斥标记，避免扫码回车和按钮重复下单。 */
  submitting = false;
  /** 开门请求互斥标记。 */
  doorPending = false;
  /** 已发送开门授权的任务编号，防止重复点击。 */
  authorizedDoorIds: number[] = [];
  /** 任务清单加载标记。 */
  loading = false;
  /** 写操作错误，只在实际失败时展示。 */
  actionError = '';
  /** 查询失败提示，不自动退出登录。 */
  readError = '';
  /** 已受理操作提示，不把它当作设备完成状态。 */
  notice = '';
  /** 服务端配置的实际柜门清单。 */
  stations: SampleCell[] = [];
  /** 柜门清单读取错误。 */
  stationError = '';
  /** 当前库存页。 */
  inventory: SampleCell[] = [];
  /** 库存库位筛选条件。 */
  inventoryFilter = '';
  /** 库存页码，从 1 开始。 */
  inventoryPage = 1;
  /** 库存总条数。 */
  inventoryTotal = 0;
  /** 库存请求是否在执行。 */
  inventoryLoading = false;
  /** 库存读取错误，出现时禁止使用旧列表选择实物。 */
  inventoryError = '';
  /** 生命周期标记，离开页面后不再更新或安排轮询。 */
  private alive = false;
  /** 同一页面的请求取消令牌。 */
  private cancellation: CancelTokenSource | null = null;
  /** 递归定时器，只在上轮查询完成后启动。 */
  private pollTimer: any = null;
  /** 合并重复任务查询；写操作完成后会等待旧查询并重新读取。 */
  private taskRequest: Promise<void> | null = null;
  /** 库存请求序号，防止较慢的旧查询覆盖新的查询结果。 */
  private inventorySequence = 0;

  /** 当前操作名称。 */
  get operation() { return this.direction === 'in' ? '留样' : '取样'; }
  /** 已登录会话的可读账号。 */
  get userName() { return this.$store.state.session.user ? this.$store.state.session.user.userName : ''; }
  /** 仅从实际任务集合取得当前任务。 */
  get selectedTask(): SampleTask | null { return this.tasks.find(task => task.id === this.selectedTaskId) || null; }
  /** 读取失败时不继续展示过期的完成进度。 */
  get currentStep() { return this.tasksFresh ? sampleTaskStep(this.selectedTask, this.direction) : -1; }
  /** 按已确认状态展示进度摘要。 */
  get progressSummary() {
    if (!this.tasksFresh) return this.readError ? '任务状态暂未确认' : '正在读取任务';
    return this.selectedTask ? '任务 ' + this.selectedTask.id + ' · ' + sampleTaskLabel(this.selectedTask) : '等待' + this.operation + '任务';
  }
  /** 保留全部活动任务及最近十条结束任务。 */
  get taskRows() { return this.tasks.filter(task => !sampleTaskEnded(task)).concat(this.tasks.filter(sampleTaskEnded).slice(0, 10)); }
  /** 开门必须有新鲜的活动任务，等待下发或异常任务不接受操作。 */
  get canOpenDoor() {
    const task = this.selectedTask;
    return !this.wmsOnly && this.tasksFresh && !!task && !sampleTaskEnded(task)
      && ['WaitingExecute', 'Error', 'Unknown'].indexOf(task.taskStatusString) < 0
      && this.authorizedDoorIds.indexOf(task.id) < 0;
  }
  /** 步骤描述只表达业务阶段，不推测具体 PLC 动作完成。 */
  get steps() {
    return this.direction === 'in'
      ? [{ title: '预约待下发', icon: 'md-barcode' }, { title: '设备接单', icon: 'md-log-in' },
        { title: '入库执行', icon: 'ios-albums' }, { title: '等待完成', icon: 'md-download' }, { title: '留样完成', icon: 'ios-flag' }]
      : [{ title: '选择样品', icon: 'md-search' }, { title: '设备接单', icon: 'md-log-out' },
        { title: '出库执行', icon: 'ios-albums' }, { title: '柜门与完成确认', icon: 'md-open' }, { title: '取样完成', icon: 'ios-flag' }];
  }

  /** 步骤文案仅以当前服务端状态为依据。 */
  stepCaption(index: number) {
    if (this.currentStep < 0) return '等待状态确认';
    if (index < this.currentStep || (index === 4 && this.currentStep === 4)) return '已完成';
    return index === this.currentStep ? '当前阶段' : '待开始';
  }
  /** 模板使用的状态文案。 */
  taskLabel(task: SampleTask) { return sampleTaskLabel(task); }
  /** 选择任务只切换查看对象，不发送设备指令。 */
  selectTask(task: SampleTask | null) { if (task) this.selectedTaskId = task.id; }
  /** 柜门只关联当前方向的活动任务，不从视觉序号生成编码。 */
  taskForStation(station: SampleCell): SampleTask | null {
    return this.tasks.find(task => !sampleTaskEnded(task) &&
      (this.direction === 'in' ? task.startCellCode : task.endCellCode) === station.cellCode) || null;
  }
  /** 先等待正在执行的查询；写请求后强制再读一次，避免使用写入前的快照。 */
  async loadTasks(afterMutation = false) {
    if (this.taskRequest) { await this.taskRequest; if (!afterMutation) return; }
    if (!this.alive) return;
    this.loading = true;
    this.taskRequest = (async () => {
      try {
        const tasks = await readSampleTasks(this.cancellation!.token);
        if (!this.alive) return;
        const previous = this.tasks;
        const trackedBarcode = this.awaitingBarcode || (this.selectedTask && this.selectedTask.materialBarcode);
        const trackedId = this.awaitingBarcode ? null : this.selectedTaskId;
        if (trackedBarcode && !tasks.some(task => task.materialBarcode === trackedBarcode &&
          (!trackedId || task.id === trackedId) && sampleTaskMatches(task, this.direction))) {
          const history = await readSampleTaskHistory(trackedBarcode, this.cancellation!.token);
          if (!this.alive) return;
          const matched = history.filter(task => (!trackedId || task.id === trackedId) && sampleTaskMatches(task, this.direction));
          if (!matched.length) throw new Error('当前任务暂未查到，请刷新或在 WMS 中核对结果。');
          tasks.push(...matched);
        }
        // 只保留已从服务端确认的近期结束记录；清单中消失本身不代表完成。
        const byId = new Map<number, SampleTask>();
        previous.filter(sampleTaskEnded).slice(0, 10).forEach(task => byId.set(task.id, task));
        tasks.filter(task => sampleTaskMatches(task, this.direction)).forEach(task => byId.set(task.id, task));
        this.tasks = Array.from(byId.values()).sort((a, b) => b.id - a.id);
        this.tasksFresh = true; this.readError = '';
        const accepted = this.awaitingBarcode && this.tasks.find(task => task.materialBarcode === this.awaitingBarcode);
        if (accepted) { this.selectedTaskId = accepted.id; this.awaitingBarcode = ''; }
        else if (!this.selectedTask) {
          const active = this.tasks.find(task => !sampleTaskEnded(task));
          this.selectedTaskId = active ? active.id : null;
        }
        // 出库真正结束后重新读取库存，避免仍可选择已经取出的旧实物。
        if (this.direction === 'out' && this.tasks.some(task => task.taskStatusString === 'Complete' &&
          previous.some(old => old.id === task.id && old.taskStatusString !== 'Complete'))) await this.loadInventory();
      } catch (error) {
        if (this.alive && !axios.isCancel(error)) { this.tasksFresh = false; this.readError = sampleWorkflowError(error); }
      }
    })();
    try { await this.taskRequest; } finally { this.taskRequest = null; if (this.alive) this.loading = false; }
  }
  /** 读取实际柜门清单，失败不会退回硬编码设备编号。 */
  async loadStations() {
    try {
      const stations = await readSampleStations(this.cancellation!.token);
      if (this.alive) { this.stations = stations; this.stationError = ''; }
    } catch (error) { if (this.alive && !axios.isCancel(error)) this.stationError = sampleWorkflowError(error); }
  }
  /** 每次查询完成后再安排下一次，保持单次任务轮询。 */
  async poll() {
    await this.loadTasks();
    if (this.alive) this.pollTimer = setTimeout(() => this.poll(), 2000);
  }
  /** 手动刷新保留用户所选任务，不把查询失败当作身份失效。 */
  async refresh() { await Promise.all([this.loadTasks(true), this.loadStations()]); }
  /** 选择已有预约后使用相同的完整条码扫码流程下发。 */
  async dispatchReservation() {
    if (!this.selectedTask) return;
    this.barcode = this.selectedTask.materialBarcode;
    await this.submit();
  }
  /** 只发送一次业务写请求，成功后查询真实任务；失败时不推进阶段。 */
  async submit() {
    if (this.submitting || this.wmsOnly || !this.tasksFresh || !this.alive) return;
    this.submitting = true; this.actionError = ''; this.notice = '';
    const barcode = sampleBarcode(this.barcode);
    try {
      const duplicate = this.tasks.find(task => !sampleTaskEnded(task) && task.taskStatusString !== 'WaitingExecute' &&
        (barcode ? task.materialBarcode === barcode : this.direction === 'out' && task.startCellCode === this.cellCode.trim()));
      if (duplicate) { this.selectedTaskId = duplicate.id; throw new Error('该样品已有执行中的任务，请查看进度，勿重复下发。'); }
      if (this.direction === 'in') await dispatchSampleIn(barcode, this.cancellation!.token);
      else {
        const task = await dispatchSampleOut(barcode, this.cellCode, this.cancellation!.token);
        if (!this.alive) return;
        this.selectedTaskId = task.id;
        this.awaitingBarcode = task.materialBarcode;
      }
      if (!this.alive) return;
      if (this.direction === 'in') this.awaitingBarcode = barcode;
      this.notice = this.operation + '任务已受理，正在读取实际进度。';
      await this.loadTasks(true);
      if (this.direction === 'out' && this.alive) await this.loadInventory();
    } catch (error) {
      if (this.alive && !axios.isCancel(error)) this.actionError = sampleWorkflowError(error);
    } finally { if (this.alive) this.submitting = false; }
  }
  /** 显式按所选任务授权开门，响应成功后仍等待真实状态，不自行设置“已打开”。 */
  async openDoor() {
    if (!this.canOpenDoor || this.doorPending || this.submitting || !this.selectedTask) return;
    const taskId = this.selectedTask.id;
    this.doorPending = true; this.actionError = '';
    try {
      await authorizeSampleDoor(taskId, this.cancellation!.token);
      if (!this.alive) return;
      this.authorizedDoorIds.push(taskId);
      this.notice = '柜门操作请求已发送，请以实际设备状态为准。';
      await this.loadTasks(true);
    } catch (error) { if (this.alive && !axios.isCancel(error)) this.actionError = sampleWorkflowError(error); }
    finally { if (this.alive) this.doorPending = false; }
  }
  /** 从库存行复制完整条码及真实库位；选择本身不下单。 */
  chooseInventory(cell: SampleCell) {
    if (this.wmsOnly || this.submitting || this.inventoryError || !cell.materialBarcode) return;
    this.barcode = cell.materialBarcode; this.cellCode = cell.cellCode; this.actionError = '';
  }
  /** 并发库存请求只允许最新序号更新结果。 */
  async loadInventory() {
    const sequence = ++this.inventorySequence;
    this.inventoryLoading = true;
    try {
      const page = await readSampleInventory(this.inventoryFilter, this.inventoryPage, this.cancellation!.token);
      if (this.alive && sequence === this.inventorySequence) {
        this.inventory = page.items; this.inventoryTotal = page.totalCount; this.inventoryError = '';
      }
    } catch (error) {
      if (this.alive && sequence === this.inventorySequence && !axios.isCancel(error)) this.inventoryError = sampleWorkflowError(error);
    } finally { if (this.alive && sequence === this.inventorySequence) this.inventoryLoading = false; }
  }
  /** 新筛选从第一页开始。 */
  async searchInventory() { this.inventoryPage = 1; await this.loadInventory(); }
  /** 库存分页由已知总数约束。 */
  async changeInventoryPage(offset: number) {
    if (this.inventoryLoading || this.inventoryPage + offset < 1 || (offset > 0 && this.inventoryPage * 10 >= this.inventoryTotal)) return;
    this.inventoryPage += offset; await this.loadInventory();
  }
  /** 返回首页清除当前终端登录凭据，下次业务仍需重新验证。 */
  returnHome() {
    this.$store.commit('app/logout', this); Util.abp.auth.clearToken();
    Cookies.set('facing', '0'); Cookies.set('locking', '0'); Cookies.set('userId', '0');
    this.$router.push({ path: '/' });
  }
  /** 注册可取消查询并启动顺序轮询。 */
  created() {
    this.alive = true; this.cancellation = axios.CancelToken.source();
    this.poll(); this.loadStations();
    if (this.direction === 'out') this.loadInventory();
  }
  /** 取消迟到响应及定时器，避免离开页面后继续轮询或改变任务展示。 */
  beforeDestroy() {
    this.alive = false; clearTimeout(this.pollTimer); this.inventorySequence++;
    if (this.cancellation) this.cancellation.cancel('已离开样品业务页面');
  }
}
</script>

<style lang="less">
@import './stockin/sample-retention.less';
</style>
<style scoped lang="less">
.sample-workflow { color: #2b405e; }
.workflow-header { display: flex; align-items: center; justify-content: space-between; gap: 16px; padding: 14px 28px; background: #205fa9; color: #fff; }
.workflow-header h1 { font-size: 26px; }.workflow-header > div { display: flex; align-items: center; gap: 18px; }
.workflow-content { max-width: 1540px; margin: auto; padding: 20px; display: grid; gap: 18px; }
.workflow-content .sample-progress { margin-top: 0; }
.workflow-panel { padding: 20px 24px; border: 1px solid #aab9cc; border-radius: 16px; background: #f7f9fc; min-width: 0; }
.workflow-panel h2 { margin-bottom: 12px; font-size: 20px; }.workflow-section-title { display: flex; justify-content: space-between; align-items: center; gap: 12px; }
.workflow-form { display: flex; align-items: flex-end; flex-wrap: wrap; gap: 16px; }.workflow-form label { display: grid; gap: 7px; flex: 1 1 260px; }
input { width: 100%; min-height: 46px; padding: 10px 12px; border: 1px solid #8ca3be; border-radius: 8px; font: inherit; color: #243c58; background: white; }
button { min-height: 44px; padding: 9px 17px; border: 1px solid #7396c2; border-radius: 8px; font: inherit; background: #e3edf9; color: #214c83; cursor: pointer; touch-action: manipulation; }
button[type='submit'] { background: #2465b2; color: white; }button:disabled { cursor: default; opacity: .55; }button:focus, input:focus { outline: 3px solid #98b9e7; outline-offset: 2px; }
.workflow-info, .workflow-hint { margin-top: 8px; color: #52657d; line-height: 1.7; }.workflow-info { margin: 0; padding: 12px 18px; background: #eaf0f9; border-radius: 10px; }
.workflow-error { margin-top: 12px; padding: 12px 15px; color: #a52c32; background: #fff0f0; border: 1px solid #e3b2b4; border-radius: 8px; overflow-wrap: break-word; }
.workflow-notice { margin-top: 12px; padding: 12px 15px; background: #e7f3ed; border-radius: 8px; color: #285f4c; }
.workflow-table-scroll { overflow-x: auto; margin-top: 12px; }table { width: 100%; border-collapse: collapse; text-align: left; }th, td { padding: 12px 10px; border-bottom: 1px solid #d2dce9; overflow-wrap: anywhere; }th { color: #425a77; white-space: nowrap; }
.workflow-selected { background: #e2edf9; }.workflow-task-actions { margin-top: 16px; display: flex; flex-wrap: wrap; align-items: center; gap: 14px; overflow-wrap: anywhere; }
.workflow-pagination { display: flex; justify-content: flex-end; align-items: center; gap: 14px; margin-top: 12px; }
.workflow-stations { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 12px; margin-top: 16px; }.workflow-stations button { min-height: 116px; display: grid; align-content: center; gap: 5px; }
@media (max-width: 650px) { .workflow-header { padding: 12px 16px; }.workflow-content { padding: 12px; }.workflow-panel { padding: 16px; }.workflow-form button { width: 100%; } }
</style>
