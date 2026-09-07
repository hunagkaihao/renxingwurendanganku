<template>
  <div>
    <BasicTable @register="registerTable" size="small">
      <template #toolbar>
        <a-button
          preIcon="ant-design:plus-circle-outlined"
          type="primary"
          @click="openCreateCellModal"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          {{ t('common.createText') }}
        </a-button>
      
        <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="createCellBat"
            v-auth="'WarehouseManagement.CellManagement.Create'"
          >
          {{ t('密集架库位初始化') }}
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
              auth: 'WarehouseManagement.CellManagement.Update',
              label: t('common.editText'),
              onClick: handleEdit.bind(null, record),
            },
            
          ]"
          :dropDownActions="[
            {
              auth: 'WarehouseManagement.CellManagement.Delete',
              label: t('common.delText'),
              onClick: handleDelete.bind(null, record),
            },
            {
              auth: 'WarehouseManagement.CellManagement.Delete',
              label: t('启用'),
              onClick: handleEnable.bind(null, record),
            },
            {
              auth: 'WarehouseManagement.CellManagement.Delete',
              label: t('禁用'),
              onClick: handleDisable.bind(null, record),
            },
          ]"
        />
      </template>
    </BasicTable>
    <CreateCell
      @register="registerCreateCellModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
    <CreateCellBat
      @register="registercreateCellBatModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
    <EditCell
      @register="registerEditCellModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"
    />
  </div>
</template>

<script lang="ts">
  import { defineComponent, reactive } from 'vue';
  import { useMessage } from '/@/hooks/web/useMessage';
  import { BasicTable, useTable, TableAction } from '/@/components/Table';
  import { tableColumns, searchFormSchema, getTableListAsync, deleteCellAsync,setCellDisable,setCellEnable } from './Cell';
  import { useModal } from '/@/components/Modal';
  import CreateCell from './CreateCell.vue';
  import CreateCellBat from './CreateCellBat.vue';
  import EditCell from './EditCell.vue';
  import { message } from 'ant-design-vue';
  import { useI18n } from '/@/hooks/web/useI18n';
  import{ useUserStore } from '/@/store/modules/user'
  import { Tag } from 'ant-design-vue';
  
  export default defineComponent({
    name: 'Cell',
    components: {
      BasicTable,
      TableAction,
      CreateCell,
      CreateCellBat,
      EditCell,
      Tag,
    },
    setup() {
      const { createConfirm } = useMessage();
      const { t } = useI18n();
      const [registerCreateCellModal, { openModal: openCreateCellModal }] = useModal();
      const [registercreateCellBatModal, { openModal: createCellBat }] = useModal();
      const cellStore = useUserStore()
      const [registerEditCellModal, { openModal: openEditCellModal }] = useModal();
      //console.log(cellStore.getWare)

    //   const searchFormSchema: FormSchema[] = reactive([
    //   {
    //     field: 'filter',
    //     label: t('routes.warehouse.cellManagement_cellCode'),
    //     component: 'Input',
    //     colProps: { span: 8 },
    //   },
    //   {
    //     field: 'Warehouseld',
    //     label: t('所属仓库'),
    //     component: 'Select',
    //     defaultValue:cellStore.getCell,
    //     colProps: { span: 8 },
    //     componentProps:{
    //       options: [
    //     {
    //       label: cellStore.getWare[0].warename,
    //       value: cellStore.getWare[0].wareid,
    //     },
    //     {
    //       label: cellStore.getWare[1].warename,
    //       value: cellStore.getWare[1].wareid,
    //     },
    //     {
    //       label: cellStore.getWare[2].warename,
    //       value: cellStore.getWare[2].wareid,
    //     },
    //   ],
    //     }
    //   },
    // ]);
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
        actionColumn: {
          width: 120,
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
        openEditCellModal(true, {
          record: record,
        });
      };

      // 删除库位
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
              await deleteCellAsync({ id: record.id, reload });
            },
          });
        }
      };
      // 启用库位
      const handleEnable = async (record: Recordable) => {
        if (record.name == 'admin') {
          message.error('admin not delete');
          return;
        } else {
          let msg = t('确认启用么？');
          createConfirm({
            iconType: 'warning',
            title: t('common.tip'),
            content: msg,
            onOk: async () => {
              await setCellEnable({ id: record.cellCode, reload });
            },
          });
        }
      };
      // 禁用库位
      const handleDisable = async (record: Recordable) => {
        if (record.name == 'admin') {
          message.error('admin not delete');
          return;
        } else {
          let msg = t('确认禁用么？');
          createConfirm({
            iconType: 'warning',
            title: t('common.tip'),
            content: msg,
            onOk: async () => {
              await setCellDisable({ id: record.cellCode, reload });
            },
          });
        }
      };

      return {
        registerTable,
        handleEdit,
        handleDelete,
        handleEnable,
        handleDisable,
        registerCreateCellModal,
        registercreateCellBatModal,
        openCreateCellModal,
        createCellBat,
        registerEditCellModal,
        t,
        reload,
      };
    },
  });
</script>
