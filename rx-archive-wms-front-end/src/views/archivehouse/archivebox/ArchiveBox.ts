import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import { SelectItem } from '/@/utils/SelectItem';
import moment from 'moment';
import {
  ArchiveBoxsServiceProxy,
  StockTasksServiceProxy,
  CreateStockTaskDto,
  PagingArchiveBoxListInput,
  PagingArchiveBoxDetailInput,
  ArchiveBoxDtoPagedResultDto,
  ArchiveBoxDetailDtoPagedResultDto,
  CreateArchiveBoxDto,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';

import { useI18n } from '/@/hooks/web/useI18n';
const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});

export const tableColumns: BasicColumn[] = [
  {
    title: t('档案盒标签'),
    dataIndex: 'archiveBoxRfid',
  },
  {
    title: t('档案盒名称'),
    dataIndex: 'archiveBoxName',
  },
  {
    title: t('档号'),
    dataIndex: 'stockBarcode',
  },
  {
    title: t('库位'),
    dataIndex: 'cellCode',
  },
  {
    title: t('尺寸'),
    dataIndex: 'cellModel',
    customRender: ({ text }) => {
      if (text != undefined) return cellModelSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('年度'),
    dataIndex: 'year',
  },
  {
    title: t('密级'),
    dataIndex: 'secretLevel',
  },
  {
    title: t('保存期限'),
    dataIndex: 'retentionPeriod',
  },
  {
    title: t('目录号'),
    dataIndex: 'catalogNo',
  },
  {
    title: t('routes.warehouse.storageBoxManagement_createTime'),
    dataIndex: 'creationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
];
export const secretLevelSelectItem: SelectItem[] = [
  {
    label: '保密',
    value: '保密',
    key: 1,
  },
  {
    label: '非保密',
    value: '非保密',
    key: 2,
  },
];
export const cellModelSelectItem: SelectItem[] = [
  {
    label: '3英寸',
    value: 'Inch3',
    key: 0,
  },
  {
    label: '4英寸',
    value: 'Inch4',
    key: 1,
  },
];

export const tableDetailColumns: BasicColumn[] = [
  {
    title: t('档案号'),
    dataIndex: 'archiveCode',
  },
  {
    title: t('档案名称'),
    dataIndex: 'archiveName',
  },
];

export const searchFormSchema: FormSchema[] = [
  {
    field: 'filter',
    label: t('关键字'),
    component: 'Input',
    colProps: { span: 12 },
  },
];

export const createFormSchema: FormSchema[] = [
  {
    field: 'archiveBoxRfid',
    component: 'Input',
    label: t('档案盒条码'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'archiveBoxName',
    component: 'Input',
    label: t('档案盒名'),
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
    field: 'stockBarcode',
    component: 'Input',
    label: t('档号'),
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
    field: 'cellModel',
    component: 'Select',
    label: t('尺寸'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      options: cellModelSelectItem,
    },
  },
  {
    field: 'year',
    component: 'Input',
    label: t('年度'),
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
    field: 'secretLevel',
    component: 'Select',
    label: t('密级'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      options: secretLevelSelectItem,
    },
  },

  {
    field: 'retentionPeriod',
    component: 'Input',
    label: t('保管期限'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'catalogNo',
    component: 'Input',
    label: t('目录号'),
    labelWidth: 85,
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
    field: 'archiveBoxRfid',
    component: 'Input',
    label: t('档案盒条码'),
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
    field: 'archiveBoxName',
    component: 'Input',
    label: t('档案盒名'),
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
    field: 'stockBarcode',
    component: 'Input',
    label: t('档号'),
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
    field: 'cellModel',
    component: 'Select',
    label: t('尺寸'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      options: cellModelSelectItem,
    },
  },
  {
    field: 'year',
    component: 'Input',
    label: t('年度'),
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
    field: 'secretLevel',
    component: 'Select',
    label: t('密级'),
    labelWidth: 85,
    // required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      options: secretLevelSelectItem,
    },
  },
  {
    field: 'retentionPeriod',
    component: 'Input',
    label: t('保管期限'),
    labelWidth: 85,
    // required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'catalogNo',
    component: 'Input',
    label: t('目录号'),
    labelWidth: 85,
    // required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];

/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync(
  params: PagingArchiveBoxListInput
): Promise<ArchiveBoxDtoPagedResultDto> {
  if (params.filter == undefined) {
    params.filter = '';
  }
  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  return _ArchiveBoxsServiceProxy.page(params);
}

/**
 * 分页明细列表
 * @param params
 * @returns
 */
export async function getDetaiTableListAsync(
  params: PagingArchiveBoxDetailInput
): Promise<ArchiveBoxDetailDtoPagedResultDto> {
  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  return _ArchiveBoxsServiceProxy.pageDetail(params);
}

/**
 * 创建书籍
 * @param param0
 */
export async function createStorageBoxAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  await _ArchiveBoxsServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}

/**
 * 删除用户
 * @param param0
 */
export async function deleteStorageBoxAsync({ id, reload }) {
  try {
    const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
    openFullLoading();
    const request = new CreateArchiveBoxDto();
    request.id = id;
    await _ArchiveBoxsServiceProxy.delete(request);
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
export async function updateStorageBoxAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  await _ArchiveBoxsServiceProxy.update(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
//创建档案入库
export async function createWCSIn({ id, reload }) {
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    const request = new CreateStockTaskDto();
    request.archiveBoxId = id;
    await _stockTasksServiceProxy.createWCSIn(request);
    closeFullLoading();
    message.success(t('common.operationSuccess'));
    reload();
  } catch (error) {
    closeFullLoading();
  }
}
//创建档案出库
export async function createWCSOut({ id, reload }) {
  try {
    const _stockTasksServiceProxy = new StockTasksServiceProxy();
    openFullLoading();
    const request = new CreateStockTaskDto();
    request.archiveBoxId = id;
    await _stockTasksServiceProxy.createWCSOut(request);
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

  const _goodssServiceProxy = new StockTasksServiceProxy();
  await _goodssServiceProxy.update(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}

/**
 * 编辑用户
 * @param param0
 */
export async function blindBoxAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  await _ArchiveBoxsServiceProxy.bindArchive(request.archiveBoxRfid, request.archiveRfid);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
/**
 * 编辑用户
 * @param param0
 */
export async function blindRfidAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _ArchiveBoxsServiceProxy = new ArchiveBoxsServiceProxy();
  await _ArchiveBoxsServiceProxy.bindRfid(request);
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
export const blindBoxFormSchema: FormSchema[] = [
  {
    field: 'archiveBoxName',
    component: 'Input',
    label: t('档案盒名称'),
    labelWidth: 85,
    colProps: {
      span: 24,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'archiveBoxRfid',
    component: 'Input',
    label: t('标签条码'),
    labelWidth: 85,
    colProps: {
      span: 24,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];
export const bindArchiveFormSchema: FormSchema[] = [
  {
    field: 'archiveBoxRfid',
    component: 'Input',
    label: t('档案盒条码'),
    labelWidth: 85,
    colProps: {
      span: 24,
    },
    componentProps: {
      autocomplete: 'off',
      disabled: true,
    },
  },
  {
    field: 'archiveRfid',
    component: 'Input',
    label: t('档案条码'),
    labelWidth: 85,
    colProps: {
      span: 24,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
];
export async function createManyGoodsAsync({ request, changeOkLoading, closeModal }) {
  changeOkLoading(true);
  // await validate();
  const _goodssServiceProxy = new ArchiveBoxsServiceProxy();
  await _goodssServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  // resetFields();
  closeModal();
}
