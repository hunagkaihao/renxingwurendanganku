import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
// import moment from 'moment';
import {
  GoodssServiceProxy,
  PagingGoodsListInput,
  GoodsDtoPagedResultDto,
  IdIntInput,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';

import { useI18n } from '/@/hooks/web/useI18n';
const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});

export const tableColumns: BasicColumn[] = [
  // {
  //   title: t('ID'),
  //   dataIndex: 'id',
  // },
  {
    title: t('分类号'),
    dataIndex: 'goodsCode',
  },
  {
    title: t('分类名称'),
    dataIndex: 'goodsName',
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
    field: 'goodsCode',
    component: 'Input',
    label: t('分类号'),
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
    label: t('分类名称'),
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
    field: 'goodsCode',
    component: 'Input',
    label: t('分类号'),
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
    label: t('分类名称'),
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
/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync(
  params: PagingGoodsListInput
): Promise<GoodsDtoPagedResultDto> {
  const _goodssServiceProxy = new GoodssServiceProxy();
  return _goodssServiceProxy.page(params);
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
