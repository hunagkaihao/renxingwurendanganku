import type { BasicColumn, FormSchema } from '/@/components/Table';
import {
  createMaterial,
  deleteMaterial,
  getMaterialPage,
  updateMaterial,
  type CreateMaterialDto,
  type PagingMaterialListInput,
} from '/@/api/material';
import { message } from 'ant-design-vue';
import { useLoading } from '/@/components/Loading';
import { useI18n } from '/@/hooks/web/useI18n';

const { t } = useI18n();
const [openFullLoading, closeFullLoading] = useLoading({ tip: 'Loading...' });

/** 物料基础信息列表列定义。序号由 BasicTable 的 showIndexColumn 生成。 */
export const tableColumns: BasicColumn[] = [
  { title: '物料码', dataIndex: 'materialCode' },
  { title: '物料名称', dataIndex: 'materialName' },
  { title: '物料类型', dataIndex: 'materialType' },
  { title: '单位', dataIndex: 'materialUnit' },
  { title: '有效期（天）', dataIndex: 'validityDays' },
  { title: '创建用户', dataIndex: 'creatorUserCode' },
  {
    title: '创建时间',
    dataIndex: 'materialCreateTime',
    customRender: ({ text }) => text?.replace('T', ' '),
  },
];

/** 物料基础信息查询条件。 */
export const searchFormSchema: FormSchema[] = [
  { field: 'filter', label: '关键字', component: 'Input', colProps: { span: 6 } },
];

/** 新增物料基础信息表单。 */
export const createFormSchema: FormSchema[] = [
  {
    field: 'materialCode', component: 'Input', label: '物料码', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialName', component: 'Input', label: '物料名称', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialType', component: 'Input', label: '物料类型', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialUnit', component: 'Input', label: '单位', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'validityDays', component: 'InputNumber', label: '有效期（天）', labelWidth: 100,
    required: true, colProps: { span: 12 }, componentProps: { min: 0, precision: 0 },
  },
  {
    field: 'creatorUserCode', component: 'Input', label: '创建用户', labelWidth: 100, required: true,
    colProps: { span: 12 }, componentProps: { autocomplete: 'off' },
  },
  {
    field: 'materialCreateTime', component: 'DatePicker', label: '创建时间', labelWidth: 100,
    required: true, colProps: { span: 12 }, componentProps: {
      showTime: true, format: 'YYYY-MM-DD HH:mm:ss', valueFormat: 'YYYY-MM-DDTHH:mm:ss',
    },
  },
];

/** 编辑表单与新增表单使用相同的物料基础信息字段。 */
export const editFormSchema = createFormSchema;

/** 查询物料基础信息列表。 */
export const getTableListAsync = (params: PagingMaterialListInput) => getMaterialPage(params);

/** 新增物料并关闭表单。 */
export async function createMaterialAsync({ request, changeOkLoading, validate, closeModal, resetFields }) {
  changeOkLoading(true);
  try {
    await validate();
    await createMaterial(request as CreateMaterialDto);
    message.success(t('common.operationSuccess'));
    resetFields();
    closeModal();
  } finally {
    changeOkLoading(false);
  }
}

/** 更新物料并关闭表单。 */
export async function updateMaterialAsync({ request, changeOkLoading, validate, closeModal, resetFields }) {
  changeOkLoading(true);
  try {
    await validate();
    await updateMaterial(request as CreateMaterialDto);
    message.success(t('common.operationSuccess'));
    resetFields();
    closeModal();
  } finally {
    changeOkLoading(false);
  }
}

/** 删除物料并刷新列表。 */
export async function deleteMaterialAsync({ id, reload }) {
  openFullLoading();
  try {
    await deleteMaterial(id);
    message.success(t('common.operationSuccess'));
    reload();
  } finally {
    closeFullLoading();
  }
}
