<template>
  <BasicModal
    :width="900"
    title="Excel 导入物料基础信息"
    :canFullscreen="false"
    :destroyOnClose="true"
    :maskClosable="false"
    @ok="submit"
    @cancel="closeModal"
    @register="registerModal"
  >
    <p class="mb-3 text-secondary">
      Excel 第一行请使用：物料码、物料名称、物料类型、单位、有效期（天）、创建用户、创建时间。
    </p>
    <ImpExcel @success="loadDataSuccess">
      <a-button class="mb-3" type="primary">选择 Excel 文件</a-button>
    </ImpExcel>
    <BasicTable :columns="tableColumns" :dataSource="importRows" :pagination="false" :scroll="{ y: 360 }" />
  </BasicModal>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import * as XLSX from 'xlsx';
  import { BasicModal, useModalInner } from '/@/components/Modal';
  import { ImpExcel, type ExcelData } from '/@/components/Excel';
  import { BasicTable, type BasicColumn } from '/@/components/Table';
  import { message } from 'ant-design-vue';
  import { createMaterial, type CreateMaterialDto } from '/@/api/material';

  interface ImportMaterialRow extends CreateMaterialDto {
    rowNumber: number;
  }

  const tableColumns: BasicColumn[] = [
    { title: '行号', dataIndex: 'rowNumber', width: 80 },
    { title: '物料码', dataIndex: 'materialCode' },
    { title: '物料名称', dataIndex: 'materialName' },
    { title: '物料类型', dataIndex: 'materialType' },
    { title: '单位', dataIndex: 'materialUnit' },
    { title: '有效期（天）', dataIndex: 'validityDays' },
    { title: '创建用户', dataIndex: 'creatorUserCode' },
    { title: '创建时间', dataIndex: 'materialCreateTime' },
  ];

  const getCellValue = (row: Record<string, unknown>, names: string[]) => {
    const key = Object.keys(row).find((item) => names.includes(item.trim()));
    return key ? row[key] : undefined;
  };

  const formatDateTime = (value: unknown) => {
    if (typeof value === 'number') {
      const date = XLSX.SSF.parse_date_code(value);
      if (!date) return '';
      return `${date.y}-${String(date.m).padStart(2, '0')}-${String(date.d).padStart(2, '0')}T${String(
        date.H || 0
      ).padStart(2, '0')}:${String(date.M || 0).padStart(2, '0')}:${String(date.S || 0).padStart(2, '0')}`;
    }
    if (value instanceof Date && !Number.isNaN(value.getTime())) return value.toISOString().slice(0, 19);
    const text = String(value ?? '').trim();
    if (!text) return '';
    return text.includes('T') ? text : text.replace(' ', 'T');
  };

  export default defineComponent({
    name: 'ImportMaterialExcel',
    components: { BasicModal, BasicTable, ImpExcel },
    emits: ['reload'],
    setup(_, { emit }) {
      const importRows = ref<ImportMaterialRow[]>([]);
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner(() => {
        importRows.value = [];
      });

      const loadDataSuccess = (excelDataList: ExcelData[]) => {
        const sourceRows = excelDataList.flatMap((sheet) => sheet.results as Record<string, unknown>[]);
        const parsedRows: ImportMaterialRow[] = [];

        for (const [index, row] of sourceRows.entries()) {
          const validityDays = Number(getCellValue(row, ['有效期（天）', '有效期', 'validityDays']));
          const materialCreateTime = formatDateTime(getCellValue(row, ['创建时间', 'materialCreateTime']));
          const material: ImportMaterialRow = {
            rowNumber: index + 2,
            materialCode: String(getCellValue(row, ['物料码', 'materialCode']) ?? '').trim(),
            materialName: String(getCellValue(row, ['物料名称', 'materialName']) ?? '').trim(),
            materialType: String(getCellValue(row, ['物料类型', 'materialType']) ?? '').trim(),
            materialUnit: String(getCellValue(row, ['单位', 'materialUnit']) ?? '').trim(),
            validityDays,
            creatorUserCode: String(getCellValue(row, ['创建用户', 'creatorUserCode']) ?? '').trim(),
            materialCreateTime,
          };

          if (
            !material.materialCode ||
            !material.materialName ||
            !material.materialType ||
            !material.materialUnit ||
            !Number.isInteger(validityDays) ||
            validityDays < 0 ||
            !material.creatorUserCode ||
            !materialCreateTime
          ) {
            message.error(`第 ${material.rowNumber} 行数据不完整或有效期不是非负整数。`);
            importRows.value = [];
            return;
          }
          parsedRows.push(material);
        }
        importRows.value = parsedRows;
      };

      const submit = async () => {
        if (!importRows.value.length) {
          message.warning('请先选择包含有效物料数据的 Excel 文件。');
          return;
        }
        changeOkLoading(true);
        try {
          for (const row of importRows.value) {
            const { rowNumber, ...material } = row;
            await createMaterial(material);
          }
          message.success(`成功导入 ${importRows.value.length} 条物料数据。`);
          emit('reload');
          closeModal();
        } catch (error) {
          message.error('导入失败，请检查接口响应及 Excel 数据后重试。');
        } finally {
          changeOkLoading(false);
        }
      };

      return { closeModal, importRows, loadDataSuccess, registerModal, submit, tableColumns };
    },
  });
</script>
