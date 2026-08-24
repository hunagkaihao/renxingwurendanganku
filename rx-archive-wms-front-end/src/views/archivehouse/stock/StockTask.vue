<template>
    <div>
      <BasicTable @register="registerTable"
      @selection-change="onSelectChange"
      :clickToRowSelect="true" size="small">
        <template #toolbar>
          <a-button
            type="primary"
            @click="wcsInCell"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('下达任务') }}
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
            @click="OpenDoor"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('开门指令') }}
          </a-button>
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

    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent,ref } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { tableColumns, searchFormSchema, getTableListAsync, wcsInSetCell,cancelTaskAsync,wcsOpenDoor } from './StockTask';
    import { useModal } from '/@/components/Modal';
    import { message } from 'ant-design-vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    export default defineComponent({
      name: 'StockTask',
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
        const selectedBoxIdRef = ref('');
        // table配置
        const [registerTable, { reload }] = useTable({
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
          if(selectedBoxIdRef.value == ''){
            message.error("请先选择档案盒")
            return
          }
            let msg = t('确认下达任务？');
            let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await wcsInSetCell({ id, reload });
              },
            });
        };
        const OpenDoor = async()=>{
          if(selectedBoxIdRef.value == ''){
            message.error("请先选择档案盒")
            return
          }
          let msg = t('确认手动下达开门指令？');
            let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await wcsOpenDoor({ id, reload });
              },
            });
        }

        const cancalTask = async () => {
          if(selectedBoxIdRef.value == ''){
            message.error("请先选择档案盒")
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
        } else {
          selectedBoxIdRef.value = '';
        }
        //reloadDetail();
      };
  
        return {
          onSelectChange,
          wcsInCell,
          registerTable,
          handleEdit,
          registerCreateArchiveModal,
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
          OpenDoor,
        };
      },
    });
  </script>
  