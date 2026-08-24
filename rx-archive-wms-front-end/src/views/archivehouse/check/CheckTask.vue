<template>
    <div>
      <BasicTable @register="registerTable" size="small">
        <template #toolbar>
          <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="openCreateArchiveModal"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('common.createText') }}
          </a-button>
          <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="openImportGoodssModal"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('EXCEL导入') }}
          </a-button>
          <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="openImportGoodssModal"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('EXCEL导出') }}
          </a-button>
        </template>
        <template #isActive="{ record }">
          <Tag :color="record.isActive ? 'green' : 'red'">
            {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
          </Tag>
        </template>
        <template #action="{ record }">
          <TableAction
            :actions="[
              {
                icon: 'ant-design:edit-outlined',
                auth: 'WarehouseManagement.GoodsManagement.Update',
                label: t('common.editText'),
                onClick: handleEdit.bind(null, record),
              },
            ]"
            :dropDownActions="[
              {
                auth: 'WarehouseManagement.GoodsManagement.Delete',
                label: t('common.delText'),
                onClick: handleDelete.bind(null, record),
              },
              
            ]"
          />
        </template>
      </BasicTable>

    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { tableColumns, searchFormSchema, getTableListAsync, deleteGoodsAsync } from './Check';
    import { useModal } from '/@/components/Modal';
    import { message } from 'ant-design-vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    export default defineComponent({
      name: 'ArchiveBox',
      components: {
        BasicTable,
        TableAction,
        Tag,
      },
      setup() {
        const { createConfirm } = useMessage();
        const { t } = useI18n();
        const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();
  
        const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();
  
        const [registerBlindBoxModal, { openModal: openBlindBoxModal }] = useModal();
  
        const [registerImportGoodssModal, { openModal: openImportGoodssModal }] = useModal();
  
        // table配置
        const [registerTable, { reload }] = useTable({
          columns: tableColumns,
          formConfig: {
            labelWidth: 70,
            schemas: searchFormSchema,
          },
          api: getTableListAsync,
          showTableSetting: true,
          useSearchForm: true,
          bordered: true,
          canResize: true,
          showIndexColumn: true,
          rowSelection: { type: 'radio' },
          actionColumn: {
            width: 150,
            title: t('common.action'),
            dataIndex: 'action',
            slots: {
              customRender: 'action',
            },
            fixed: 'right',
          },
        });
  
        // 编辑用户
        const handleEdit = (record: Recordable) => {
          openEditArchiveModal(true, {
            record: record,
          });
        };
  
        // 绑定容器
        const handleBlindBox = (record: Recordable) => {
          openBlindBoxModal(true, {
            record: record,
          });
        };
  
        // 删除用户
        const handleDelete = async (record: Recordable) => {
          if (record.name == 'admin') {
            message.error('admin not delete');
            return;
          } else {
            let msg = t('common.askDelete');
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await deleteGoodsAsync({ id: record.id, reload });
              },
            });
          }
        };
  
        return {
          registerTable,
          handleEdit,
          handleDelete,
          registerCreateArchiveModal,
          openCreateArchiveModal,
          registerEditArchiveModal,
          registerBlindBoxModal,
          registerImportGoodssModal,
          openImportGoodssModal,
          handleBlindBox,
          t,
          reload,
        };
      },
    });
  </script>
  