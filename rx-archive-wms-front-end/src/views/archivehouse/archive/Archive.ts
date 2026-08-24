import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import moment from 'moment';
import {
  GoodssServiceProxy,
  ArchivesServiceProxy,
  PagingStockTaskDetailInput,
  PagingArchiveListInput,
  StockTaskDetailDtoPagedResultDto,
  ArchiveDtoPagedResultDto,
  PickOutDto,
  CreateArchiveDto,
  StockTasksServiceProxy,
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
    title: t('档号'),
    dataIndex: 'archivesCode',
  },
  {
    title: t('题名'),
    dataIndex: 'archivesName',
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

export const tableDetailColumns: BasicColumn[] = [
  {
    title: t('出入库类型'),
    dataIndex: 'Type',
  },
  {
    title: t('创建人'),
    dataIndex: 'goodsSpe',
  },
  {
    title: t('借阅者'),
    dataIndex: 'goodsSpec',
  },
  {
    title: t('借阅日期'),
    dataIndex: 'cretionTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
  },
  {
    title: t('归还人'),
    dataIndex: 'godsSpec',
  },
  {
    title: t('归还日期'),
    dataIndex: 'ceationTime',
    customRender: ({ text }) => {
      return moment(text).format('YYYY-MM-DD HH:mm:ss');
    },
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

export const createFormSchema: FormSchema[] = [
  {
    field: 'archivesRfid',
    component: 'Input',
    label: t('档案标签'),
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
    field: 'archivesCode',
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
    field: 'archivesName',
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
  {
    field: 'year',
    component: 'Input',
    label: t('年度'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'secretLevel',
    component: 'Input',
    label: t('密级'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'classType',
    component: 'Input',
    label: t('类别'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
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
    field: 'goodsAJCode',
    component: 'Input',
    label: t('案卷号'),
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
    field: 'archivesRfid',
    component: 'Input',
    label: t('档案标签'),
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
    field: 'archivesCode',
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
    field: 'archivesName',
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
  {
    field: 'year',
    component: 'Input',
    label: t('年度'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'secretLevel',
    component: 'Input',
    label: t('密级'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
    },
  },
  {
    field: 'classType',
    component: 'Input',
    label: t('类别'),
    labelWidth: 85,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
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
    field: 'goodsAJCode',
    component: 'Input',
    label: t('案卷号'),
    labelWidth: 85,
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
    field: 'archiveName',
    component: 'Input',
    label: t('档案名称'),
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
/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync(
  params: PagingArchiveListInput
): Promise<ArchiveDtoPagedResultDto> {
  const _archivesServiceProxy = new ArchivesServiceProxy();
  return _archivesServiceProxy.page(params);
}

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
  const _archivesServiceProxy = new ArchivesServiceProxy();
  await _archivesServiceProxy.create(request);
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

export async function PickOut(request: PickOutDto[]) {
  // await validate();
  const _goodssServiceProxy = new StockTasksServiceProxy();
  await _goodssServiceProxy.pickOutTask(request);
  message.success(t('common.operationSuccess'));
}

/**
 * 删除用户
 * @param param0
 */
export async function deleteGoodsAsync({ id, reload }) {
  try {
    const _archivesServiceProxy = new ArchivesServiceProxy();
    openFullLoading();
    const request = new CreateArchiveDto();
    request.id = id;
    await _archivesServiceProxy.delete(request);
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
export async function updateArchivesAsync({
  request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();

  const _ArchivesServiceProxy = new ArchivesServiceProxy();
  await _ArchivesServiceProxy.update(request);
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
  // request,
  changeOkLoading,
  validate,
  closeModal,
  resetFields,
}) {
  changeOkLoading(true);
  await validate();
  changeOkLoading(false);
  resetFields();
  message.success(t('common.operationSuccess'));
  closeModal();
}
/**
 * 分页明细列表
 * @param params
 * @returns
 */
export async function getDetaiTableListAsync(
  params: PagingStockTaskDetailInput
): Promise<StockTaskDetailDtoPagedResultDto> {
  const _ArchiveBoxsServiceProxy = new StockTasksServiceProxy();
  return _ArchiveBoxsServiceProxy.pageDetailByArchiveId(params);
}
