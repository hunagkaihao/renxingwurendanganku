import { FormSchema } from '/@/components/Table';
import { SelectItem } from '/@/utils/SelectItem';
import { BasicColumn } from '/@/components/Table';
import moment from 'moment';
import {
  GoodssServiceProxy,
  StockTasksServiceProxy,
  ChecksServiceProxy,
  CheckHissServiceProxy,
  PagingPlanListInput,
  PlanDtoPagedResultDto,
  CheckHisDtoPagedResultDto,
  PagingCheckHisDto,
  CreateCheckDto,
  CheckDto,
  CheckType,
  PagingCheckDetailInput,
  CheckDetailDtoPagedResultDto,
  PagingCheckDetailHisDto,
  CheckDetailHisDtoPagedResultDto,
  IdIntInput,
  PlansServiceProxy,
  CreatePlanDto,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';

import { useI18n } from '/@/hooks/web/useI18n';
const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});
export const CheckTypeCodeSelectItem: SelectItem[] = [
  {
    label: '循环盘点',
    value: CheckType[CheckType.CircleCheck],
    key: CheckType.CircleCheck,
  },
  {
    label: '年度盘点',
    value: CheckType[CheckType.AnnualCheck],
    key: CheckType.AnnualCheck,
  },
  {
    label: 'HgStockCheck',
    value: CheckType[CheckType.HgStockCheck],
    key: CheckType.HgStockCheck,
  },
  {
    label: '区域盘点',
    value: CheckType[CheckType.AreaCodeAuto],
    key: CheckType.AreaCodeAuto,
  },
];
export const planColumns: BasicColumn[] = [
  {
    title: t('计划编号'),
    dataIndex: 'planCode',
  },
  {
    title: t('计划类型'),
    dataIndex: 'planTypeCode',
    // customRender: ({ text }) => {
    //   return CheckTypeCodeSelectItem.filter((f) => f.key == text)[0].label;
    // },
  },
  {
    title: t('区域'),
    dataIndex: 'areaCode',
  },
  {
    title: t('创建时间'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
]
export const planDetailColumns: BasicColumn[] = [
  // {
  //   title: t('盘点编号'),
  //   dataIndex: 'checkId',
  // },
  {
    title: t('任务编号'),
    dataIndex: 'manageId',
  },
  {
    title: t('档案标签'),
    dataIndex: 'stockBarcode',
  },
  {
    title: t('库位'),
    dataIndex: 'cellName',
  },
  {
    title: t('数量'),
    dataIndex: 'account',
  },
  {
    title: t('创建时间'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
]
export const hisColumns: BasicColumn[] = [
  {
    title: t('盘点计划编号'),
    dataIndex: 'checkCode',
  },
  {
    title: t('盘点类型'),
    dataIndex: 'checkType',
    customRender: ({ text }) => {
      return CheckTypeCodeSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('区域编码'),
    dataIndex: 'areaCode',
  },
  {
    title: t('创建时间'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
  // {
  //   title: t('备注'),
  //   dataIndex: 'checkStatus',
  // },
  {
    title: t('盘点状态'),
    dataIndex: 'checkStatus',
  },
  {
    title: t('完成时间'),
    dataIndex: 'finishTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
  {
    title: t('审核完成时间'),
    dataIndex: 'verifyFinishTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
]
export const hisDetailColumns: BasicColumn[] = [
  {
    title: t('库位'),
    dataIndex: 'cellName',
  },
  // {
  //   title: t('档案ID'),
  //   dataIndex: 'goodsId',
  // },
  {
    title: t('档案盒标签'),
    dataIndex: 'stockBarcode',
  },
  {
    title: t('盘点异常反馈'),
    dataIndex: 'remark',
  },
  // {
  //   title: t('档案标签'),
  //   dataIndex: 'boxBarcode',
  // },
  {
    title: t('账目数量'),
    dataIndex: 'account',
  },
  {
    title: t('实盘数量'),
    dataIndex: 'realAmount_1',
  },
  {
    title: t('差异数量'),
    dataIndex: 'profitLossAmount',
  },
  {
    title: t('审核数量'),
    dataIndex: 'verifyAmount',
  },
  {
    title: t('审核用户'),
    dataIndex: 'verifyUser',
  },
  {
    title: t('审核完成时间'),
    dataIndex: 'verifyFinishTime',
  },
]
export const tableColumns: BasicColumn[] = [
  {
    title: t('档号'),
    dataIndex: 'goodsCode',
  },
  {
    title: t('题名'),
    dataIndex: 'goodsName',
  },
  {
    title: t('条码号'),
    dataIndex: 'rfidId',
  },
  {
    title: t('库位'),
    dataIndex: 'cellName',
  },
  {
    title: t('案卷号'),
    dataIndex: 'goodsAJCode',
  },
  {
    title: t('保管期限'),
    dataIndex: 'retentionPeriod',
  },
  {
    title: t('所属档案盒'),
    dataIndex: 'archiveBoxName',
  },
  {
    title: t('档案盒条码'),
    dataIndex: 'archiveBoxRfid',
  },
  {
    title: t('年度'),
    dataIndex: 'year',
  },
  {
    title: t('类别'),
    dataIndex: 'classType',
  },
  {
    title: t('密级'),
    dataIndex: 'secretLevel',
  },
  {
    title: t('routes.material.goodsManagement_createTime'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
];

export const checkHistableColumns: BasicColumn[] = [
    {
      title: t('ID'),
      dataIndex: 'goodsCode',
    },
    {
      title: t('盘点计划编号'),
      dataIndex: 'goodsName',
    },
    {
      title: t('盘点类型'),
      dataIndex: 'rfidId',
    },
    {
      title: t('库位'),
      dataIndex: 'cellName',
    },
    {
      title: t('案卷号'),
      dataIndex: 'goodsAJCode',
    },
    {
      title: t('保管期限'),
      dataIndex: 'retentionPeriod',
    },
    {
      title: t('所属档案盒'),
      dataIndex: 'archiveBoxName',
    },
    {
      title: t('档案盒条码'),
      dataIndex: 'archiveBoxRfid',
    },
    {
      title: t('年度'),
      dataIndex: 'year',
    },
    {
      title: t('类别'),
      dataIndex: 'classType',
    },
    {
      title: t('密级'),
      dataIndex: 'secretLevel',
    },
    {
      title: t('routes.material.goodsManagement_createTime'),
      dataIndex: 'creationTime',
      customRender: ({ text }) => {
        return moment(text).format('YYYY-MM-DD HH:mm:ss');
      },
    },
  ];

export const searchFormSchema: FormSchema[] = [
    {
        field: 'filter',
        label: t('关键字:'),
        component: 'Input',
        colProps: { span: 6 },
      },
        
];
export const createPlanFormSchema : FormSchema[] = [
  {
    field: 'planType',
    component: 'Select',
    label: t('任务类型'),
    required: true,
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      //设置选项值
      options: [
        {
          label: '盘点计划',
          value: 'Check',
        },
        {
          label: '批量入库计划',
          value: 'BatIn',
        },
        {
          label: '疲劳测试',
          value: 'Battest',
        },
      ],
    },
  },
  {
    field: 'cell_x',
    component: 'Input',
    label: t('列数'),
    defaultValue:0,
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_y',
    component: 'Input',
    label: t('层数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_z',
    component: 'Input',
    label: t('排数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  }
];
export const checkhissearchFormSchema: FormSchema[] = [
    {
        field: 'filter',
        label: t('关键字:'),
        component: 'Input',
        colProps: { span: 6 },
      },
        {
            field: 'cellId',
            label: t('任务状态:'),
            //labelWidth:150,
            component: 'Select',
            defaultValue: 'All', //设置默认值
                //required: true,
                colProps: {
                span: 6,
                },
                componentProps: {
                //设置选项值
                options: [
                    {
                    label: '全部',
                    value: 'All',
                    },
                    {
                    label: '等待执行',
                    value: '0',
                    },
                    {
                    label: '执行中',
                    value: '1',
                    },
                    {
                    label: '结束',
                    value: '2',
                    },
                ],
                },
          },
      {
        field: 'time',
        component: 'RangePicker',
        label: '创建时间:',
        labelWidth: 80,
        colProps: { span: 6 },
        defaultValue: [moment().subtract(7, 'days'), moment().add(1, 'days')],
      },
];

export const createFormSchema: FormSchema[] = [
    {
        field: 'goodsCode',
        component: 'Input',
        label: t('档号'),
        labelWidth: 85,
        colProps: {
          span: 12,
        },
        componentProps: {
          autocomplete: 'off',
        },
      },
      {
        field: 'goodsName',
        component: 'Input',
        label: t('题名'),
        labelWidth: 85,
        required: true,
        colProps: {
          span: 12,
        },
        componentProps: {
          autocomplete: 'off',
        },
      },
    
];

export const editFormSchema: FormSchema[] = [
  {
    field: 'goodsCode',
    component: 'Input',
    label: t('routes.material.goodsManagement_goodsCode'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'goodsName',
    component: 'Input',
    label: t('routes.material.goodsManagement_name'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },

];

export const blindBoxFormSchema: FormSchema[] = [
  {
    field: 'goodsCode',
    component: 'Input',
    label: t('routes.material.goodsManagement_goodsCode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'goodsName',
    component: 'Input',
    label: t('routes.material.goodsManagement_name'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'goodsSpec',
    component: 'Input',
    label: t('routes.material.goodsManagement_goodsSpec'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'goodsConstProperty1',
    component: 'Input',
    label: t('routes.material.goodsManagement_goodsBand'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'storageBoxBarCode',
    component: 'Input',
    label: t('routes.warehouse.storageBoxManagement_storageBoxBarcode'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'quantity',
    component: 'Input',
    label: t('routes.warehouse.storageBoxDetailManagement_quantity'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'goodsBatchNo',
    component: 'Input',
    label: t('routes.warehouse.storageBoxDetailManagement_goodsBatchNo'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];

export const createCheckBatFormSchema: FormSchema[] =[
  {
    field: 'cell_x',
    component: 'Input',
    label: t('列数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_y',
    component: 'Input',
    label: t('层数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'cell_z',
    component: 'Input',
    label: t('排数'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  }
]
/**
 * 创建书籍
 * @param param0
 */
export async function CreateArchiveAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  const _goodssServiceProxy = new GoodssServiceProxy();
  await _goodssServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}

export async function createManyGoodsAsync({ request, changeOkLoading, closeModal }) {
  changeOkLoading(true);
  // await validate();
  const _goodssServiceProxy = new GoodssServiceProxy();
  await _goodssServiceProxy.createMany(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  // resetFields();
  closeModal();
}

/**
 * 删除用户
 * @param param0
 */
export async function deleteGoodsAsync({ id, reload }) {
  try {
    const _goodssServiceProxy = new GoodssServiceProxy();
    openFullLoading();
    const request = new IdIntInput();
    request.id = id;
    await _goodssServiceProxy.delete(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}

/**
 * 编辑用户
 * @param param0
 */
export async function updateGoodsAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _goodssServiceProxy = new GoodssServiceProxy();
  await _goodssServiceProxy.update(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}



const _checksServiceProxy = new ChecksServiceProxy();
const _checkHissServiceProxy = new CheckHissServiceProxy();
const _stockTasksServiceProxy = new StockTasksServiceProxy();
const _plansServiceProxy= new PlansServiceProxy()
//创建盘点计划
export async function Create(
  params:CreateCheckDto
  ): Promise<CheckDto> {
  return _checksServiceProxy.createWithArea(params);
}
//获取计划
export async function getTableListAsync(
  params: PagingPlanListInput
): Promise<PlanDtoPagedResultDto> {
  return _plansServiceProxy.page(params);
}
//获取盘点计划明细
export async function getTableDetailListAsync(
  params: PagingCheckDetailInput
): Promise<CheckDetailDtoPagedResultDto> {
  return _checksServiceProxy.pageDetail(params);
}
//盘点历史
export async function GetTableHis(
  params: PagingCheckHisDto
): Promise<CheckHisDtoPagedResultDto> {
  return _checkHissServiceProxy.page(params);
}
//盘点历史结果
export async function GetTableDetailHis(
  params: PagingCheckDetailHisDto
): Promise<CheckDetailHisDtoPagedResultDto> {
  return _checkHissServiceProxy.pageDetail(params);
}
//执行盘点计划
export async function Executing(
    id:number
  ): Promise<boolean> {
    const request = new IdIntInput();
    request.id = id;
  return _checksServiceProxy.checkExecute(request);
}
//取消盘点计划
export async function Delete(
  id:number
  ): Promise<void> {
    const request = new IdIntInput();
    request.id = id;
  return _plansServiceProxy.delete(request);
}
//盘点结果处理账实一致
export async function Confirm(
  id:number
  ): Promise<any> {
    var params = new IdIntInput()
    params.id = id
  return _checksServiceProxy.inventoryConfirm(params);
}
//盘点结果处理盘亏
export async function LossConfirm(
  id:number
  ): Promise<any> {
    var params = new IdIntInput()
    params.id = id
  return _checksServiceProxy.inventoryLossConfirm(params);
}
//盘点结果处理盘盈入库
export async function createSurplusIn(
  boxRfid: string , cellName: string 
  ): Promise<any> {
  return _checksServiceProxy.createSurplusIn(boxRfid,cellName);
}
//盘点结果处理盘亏出库
export async function createLossOut(
  boxRfid: string , cellName: string 
  ): Promise<any> {
  return _checksServiceProxy.createLossOut(boxRfid,cellName);
}
//盘点完成
export async function Complete(
  params:string
  ): Promise<any> {
  return _checksServiceProxy.checkComplete(params);
}
//创建计划
export async function createplanAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  var param = new CreatePlanDto()
  param.planTypeCode = request.planType
  param.areaCode = request.cell_z+'-'+ request.cell_x +'-'+ request.cell_y
  await _plansServiceProxy.createPlan(param);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}
//批量创建cell
export async function createCheckBatAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  var param = new CreateCheckDto();
  param.areaCode = request.cell_z+'-'+ request.cell_x +'-'+ request.cell_y
  await _checksServiceProxy.createWithArea(param);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}
