<template>
  <div>
    <BasicTable
      @register="registerTable"
      @selection-change="onSelectChange"
      :clickToRowSelect="true"
      size="small"
    >
      <template #toolbar>
        <a-button
          preIcon="ant-design:plus-circle-outlined"
          type="primary"
          @click="openCreateCellModal"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          {{ t('common.createText') }}
        </a-button>
<!--        <a-button
          preIcon="ant-design:link-outlined"
          type="primary"
          @click="handleBindMaterial"
          v-auth="'WarehouseManagement.CellManagement.Update'"
        >
          物料绑定
        </a-button>-->
        <a-button
          preIcon="ant-design:unlock-outlined"
          type="primary"
          @click="handleOpenDoor"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          开柜门
        </a-button>
        <a-button
          preIcon="ant-design:import-outlined"
          type="primary"
          @click="handleStockIn"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          入库
        </a-button>
        <a-button
          preIcon="ant-design:export-outlined"
          type="primary"
          @click="handleStockOut"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          出库
        </a-button>
        <a-button
          preIcon="ant-design:export-outlined"
          type="primary"
          @click="handleBorrowStockOut"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          借用出库
        </a-button>
        <a-button
          preIcon="ant-design:import-outlined"
          type="primary"
          @click="handleReturnStockIn"
          v-auth="'WarehouseManagement.CellManagement.Create'"
        >
          归还入库
        </a-button>
      
<!--        <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="createCellBat"
            v-auth="'WarehouseManagement.CellManagement.Create'"
          >
          {{ t('密集架库位初始化') }}
        </a-button>-->
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
    <BindCellMaterial @register="registerBindMaterialModal" @reload="reload" />
    <CreateStockTask @register="registerCreateStockTaskModal" @reload="reload" />
    <BorrowStockOut @register="registerBorrowStockOutModal" @reload="reload" />
    <ReturnStockIn @register="registerReturnStockInModal" @reload="reload" />
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import { useMessage } from '/@/hooks/web/useMessage';
  import { BasicTable, useTable, TableAction } from '/@/components/Table';
  import {
    tableColumns,
    searchFormSchema,
    getTableListAsync,
    deleteCellAsync,
    setCellDisable,
    setCellEnable,
    createCellStockOutAsync,
    openCellDoorAsync,
    borrowStockOutAsync,
    returnStockInAsync,
  } from './Cell';
  import { useModal } from '/@/components/Modal';
  import CreateCell from './CreateCell.vue';
  import CreateCellBat from './CreateCellBat.vue';
  import EditCell from './EditCell.vue';
  import BindCellMaterial from './BindCellMaterial.vue';
  import CreateStockTask from '../stock/CreateStockTask.vue';
  import BorrowStockOut from './BorrowStockOut.vue';
  import ReturnStockIn from './ReturnStockIn.vue';
  import { message } from 'ant-design-vue';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { Tag } from 'ant-design-vue';
  import {
    buildCellOutboundInput,
    CellStockRecord,
    validateCellStockAction,
  } from './CellStockAction';
  
  export default defineComponent({
    name: 'Cell',
    components: {
      BasicTable,
      TableAction,
      CreateCell,
      CreateCellBat,
      EditCell,
      BindCellMaterial,
      CreateStockTask,
      BorrowStockOut,
      ReturnStockIn,
      Tag,
    },
    setup() {
      const { createConfirm } = useMessage();
      const { t } = useI18n();
      const [registerCreateCellModal, { openModal: openCreateCellModal }] = useModal();
      const [registercreateCellBatModal, { openModal: createCellBat }] = useModal();
      const [registerEditCellModal, { openModal: openEditCellModal }] = useModal();
      const [registerBindMaterialModal, { openModal: openBindMaterialModal }] = useModal();
      const [registerCreateStockTaskModal, { openModal: openCreateStockTaskModal }] = useModal();
      const [registerBorrowStockOutModal, { openModal: openBorrowStockOutModal }] = useModal();
      const [registerReturnStockInModal, { openModal: openReturnStockInModal }] = useModal();
      const selectedCell = ref<CellStockRecord>();
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
        rowSelection: { type: 'radio' },
        rowKey: 'id',
        clearSelectOnPageChange: true,
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

      const onSelectChange = ({ rows }) => {
        selectedCell.value = rows.length > 0 ? rows[0] : undefined;
      };

      const handleStockIn = () => {
        const cell = selectedCell.value;
        const error = validateCellStockAction(cell, 'in');
        if (error || !cell) {
          if (error) message.error(error);
          return;
        }

        openCreateStockTaskModal(true, { record: cell });
      };

      const handleBindMaterial = () => {
        const cell = selectedCell.value;
        if (!cell) {
          message.error('请先选择库位');
          return;
        }
        if (cell.materialCode && cell.materialCode.trim()) {
          message.error('该库位已有物料，无法绑定');
          return;
        }

        openBindMaterialModal(true, { record: cell });
      };

      const handleOpenDoor = () => {
        const cell = selectedCell.value;
        if (!cell) {
          message.error('请先选择柜门库位');
          return;
        }
        if (cell.cellType !== 'Station') {
          message.error('选中的库位不是柜门类型，无法执行开门指令');
          return;
        }

        createConfirm({
          iconType: 'warning',
          title: t('common.tip'),
          content: t('确认打开选中的柜门？'),
          onOk: async () => {
            await openCellDoorAsync({ cellCode: cell.cellCode, reload });
          },
        });
      };

      const handleStockOut = () => {
        const cell = selectedCell.value;
        const error = validateCellStockAction(cell, 'out');
        if (error || !cell) {
          if (error) message.error(error);
          return;
        }

        createConfirm({
          iconType: 'warning',
          title: t('common.tip'),
          content: t('确认出库？'),
          onOk: async () => {
            await createCellStockOutAsync({
              request: buildCellOutboundInput(cell),
              reload,
            });
          },
        });
      };

      const handleBorrowStockOut = () => {
        const cell = selectedCell.value;
        const error = validateCellStockAction(cell, 'out');
        if (error || !cell) {
          if (error) message.error(error);
          return;
        }

        openBorrowStockOutModal(true, { record: cell });
      };

      const handleReturnStockIn = () => {
        openReturnStockInModal(true, { record: selectedCell.value });
      };

      return {
        onSelectChange,
        handleBindMaterial,
        handleOpenDoor,
        handleStockIn,
        handleStockOut,
        handleBorrowStockOut,
        handleReturnStockIn,
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
        registerBindMaterialModal,
        registerCreateStockTaskModal,
        registerBorrowStockOutModal,
        registerReturnStockInModal,
        t,
        reload,
      };
    },
  });
</script>
