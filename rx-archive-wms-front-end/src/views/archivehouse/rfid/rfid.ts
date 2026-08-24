import { FormSchema } from '/@/components/Table';
import { BasicColumn } from '/@/components/Table';
import { SelectItem } from '/@/utils/SelectItem';
// import moment from 'moment';
import {
  GoodssServiceProxy,
  RfidServiceProxy,
  PagingRfidListInput,
  RfidCodeDtoPagedResultDto,
  // IdIntInput,
  CreateRfidCodeDto,
} from '/@/services/ServiceProxies';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';

import { useI18n } from '/@/hooks/web/useI18n';
const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({
  tip: 'Loading...',
});
export const cellModelSelectItem: SelectItem[] = [
  {
    label: '档案盒',
    value: '2',
    key: 2,
  },
  {
    label: '档案',
    value: '1',
    key: 1,
  },
];
export const rfidEnableStatusSelectItem: SelectItem[] = [
  {
    label: '禁用',
    value: '1',
    key: 1,
  },
  {
    label: '可用',
    value: '0',
    key: 0,
  },
];
export const rfidPrintStatusSelectItem: SelectItem[] = [
  {
    label: '打印',
    value: '1',
    key: 1,
  },
  {
    label: '未打印',
    value: '0',
    key: 0,
  },
];
export const tableColumns: BasicColumn[] = [
  {
    title: t('标签编码'),
    dataIndex: 'rfidCode',
  },
  {
    title: t('标签分类'),
    dataIndex: 'rfidTypeCode',
    customRender: ({ text }) => {
      if (text != undefined) return cellModelSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  {
    title: t('可用状态'),
    dataIndex: 'status',
    customRender: ({ text }) => {
      if (text != undefined)
        return rfidEnableStatusSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  // {
  //   title: t('写卡标记'),
  //   dataIndex: 'writeStatus',
  // },
  {
    title: t('打印状态'),
    dataIndex: 'printStatus',
    customRender: ({ text }) => {
      if (text != undefined)
        return rfidPrintStatusSelectItem.filter((f) => f.value == text)[0].label;
    },
  },
  // {
  //   title: t('备注'),
  //   dataIndex: 'archiveBoxName',
  // },
  //   {
  //     title: t('routes.material.goodsManagement_createTime'),
  //     dataIndex: 'creationTime',
  //     customRender: ({ text }) => {
  //       return moment(text).format('YYYY-MM-DD HH:mm:ss');
  //     },
  //   },
];

export const searchFormSchema: FormSchema[] = [
  {
    field: 'filter',
    label: t('标签编码:'),
    component: 'Input',
    colProps: { span: 6 },
  },
];

export const createFormSchema: FormSchema[] = [
  {
    field: 'rfidCode',
    component: 'Input',
    label: t('标签编码'),
    labelWidth: 85,
    required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      autocomplete: 'off',
      placeholder: '请输入标签编码',
    },
  },
  {
    field: 'rfidTypeCode',
    component: 'Select',
    label: t('标签类型'),
    defaultValue: '2', //设置默认值
    //required: true,
    colProps: {
      span: 12,
    },
    componentProps: {
      //设置选项值
      options: [
        {
          label: '档案盒',
          value: '2',
        },
        {
          label: '档案',
          value: '1',
        },
      ],
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
/**
 * 分页列表
 * @param params
 * @returns
 */
export async function getTableListAsync(
  params: PagingRfidListInput
): Promise<RfidCodeDtoPagedResultDto> {
  const _goodssServiceProxy = new RfidServiceProxy();
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
  const _rfidServiceProxy = new RfidServiceProxy();
  await _rfidServiceProxy.create(request);
  changeOkLoading(false);
  message.success(t('common.operationSuccess'));
  resetFields();
  closeModal();
}

export async function createManyRfidAsync({ request, changeOkLoading, closeModal }) {
  changeOkLoading(true);
  // await validate();
  const _rfidServiceProxy = new RfidServiceProxy();
  await _rfidServiceProxy.createMany(request);
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
    const _rfidServiceProxy = new RfidServiceProxy();
    openFullLoading();
    const request = new CreateRfidCodeDto();
    request.id = id;

    await _rfidServiceProxy.delete(request);
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
