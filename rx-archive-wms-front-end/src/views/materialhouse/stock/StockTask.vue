<template>
    <div>
      <BasicTable @register="registerTable"
      @selection-change="onSelectChange"
      :clickToRowSelect="true" size="small">
      <template #toolbar>
          <a-button type="primary" @click="openCreateStockTaskModal(true)" v-auth="'WarehouseManagement.GoodsManagement.Create'">
            创建入库预约
          </a-button>
          <a-button
            type="primary"
            @click="wcsInCell"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            扫码确认入库
          </a-button>
          <!-- <a-button
            type="primary"
            @click="wcsInCell"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('指定库位下达') }}
          </a-button> -->
          <a-button
            type="primary"
            @click="cancalTask"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('取消任务') }}
          </a-button>
          <a-button
            type="primary"
            @click="wcspage"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('WCS管理') }}
          </a-button>
        </template>
        <template #isActive="{ record }">
          <Tag :color="record.isActive ? 'green' : 'red'">
            {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
          </Tag>
        </template>
        
      </BasicTable>
      <CreateStockTask @register="registerCreateStockTaskModal" @reload="reload" />

    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent,ref } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { tableColumns, searchFormSchema, getTableListAsync, scanAndDispatchToWCS, cancelTaskAsync } from './StockTask';
    import { useModal } from '/@/components/Modal';
    import { message } from 'ant-design-vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    import CreateStockTask from './CreateStockTask.vue';
    export default defineComponent({
      name: 'StockTask',
      components: {
        BasicTable,
        TableAction,
        Tag,
        CreateStockTask,
      },
      setup() {
        const { createConfirm } = useMessage();
        const { t } = useI18n();
        const [registerCreateArchiveModal, { openModal: openCreateArchiveModal }] = useModal();
        const [registerCreateStockTaskModal, { openModal: openCreateStockTaskModal }] = useModal();
  
        const [registerEditArchiveModal, { openModal: openEditArchiveModal }] = useModal();
  
        const [registerBlindBoxModal, { openModal: openBlindBoxModal }] = useModal();
  
        const [registerImportGoodssModal, { openModal: openImportGoodssModal }] = useModal();
        const selectedBoxIdRef = ref('');
        const selectedStockTaskRef = ref<Recordable | null>(null);
        // table配置
        const [registerTable, { reload, clearSelectedRowKeys }] = useTable({
          columns: tableColumns,
          formConfig: {
            labelWidth: 70,
            schemas: searchFormSchema,
            fieldMapToTime: [['time', ['startCreationTime', 'endCreationTime']]],
          },
          api: getTableListAsync,
          showTableSetting: true,
          useSearchForm: true,
          bordered: true,
          canResize: true,
          showIndexColumn: false,
          rowSelection: { type: 'radio' },
          rowKey: 'id',
          clearSelectOnPageChange: true,
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
  
        

        const wcsInCell = async () => {
          const selectedTask = selectedStockTaskRef.value;
          if (!selectedTask) {
            message.error("请先选择入库任务")
            return
          }
          const isStockIn = selectedTask.taskTypeCode === 'NPFullStockIn' || selectedTask.taskTypeCode === 0;
          const isWaiting = selectedTask.taskStatus === 'WaitingExecute' || selectedTask.taskStatus === 0;
          if (!isStockIn || !isWaiting) {
            message.error("请选择等待执行的入库任务")
            return
          }
          const materialBoxBarcode = selectedTask.materialBoxBarcode;
          if (!materialBoxBarcode) {
            message.error("所选入库任务缺少物料码")
            return
          }
            let msg = '确认扫码入库并下发开门授权？';
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                const success = await scanAndDispatchToWCS({ materialBoxBarcode, reload });
                if (success) {
                  clearSelectedRowKeys();
                  selectedBoxIdRef.value = '';
                  selectedStockTaskRef.value = null;
                }
              },
            });
        };
        const cancalTask = async () => {
          if(selectedBoxIdRef.value == ''){
            message.error("请先选择要取消的任务")
            return
          }
            let msg = t('确认取消任务？');
            let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await cancelTaskAsync({ id, reload });
              },
            });
        };

        function wcspage(){
          window.open('http://192.168.0.108:3271/#/orderListMonitor');
        }

        //勾选事件
        const onSelectChange = async ({ rows }) => {
          if (rows.length > 0) {
            selectedBoxIdRef.value = rows[0].id;
            selectedStockTaskRef.value = rows[0];
          } else {
            selectedBoxIdRef.value = '';
            selectedStockTaskRef.value = null;
          }
        //reloadDetail();
      };
  
        return {
          onSelectChange,
          wcsInCell,
          registerTable,
          handleEdit,
          registerCreateArchiveModal,
          registerCreateStockTaskModal,
          openCreateStockTaskModal,
          openCreateArchiveModal,
          registerEditArchiveModal,
          registerBlindBoxModal,
          registerImportGoodssModal,
          openImportGoodssModal,
          handleBlindBox,
          t,
          wcspage,
          cancalTask,
          reload,
        };
      },
    });
  </script>
