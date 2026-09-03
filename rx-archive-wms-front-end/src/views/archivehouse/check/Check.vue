<template>
    <div>
      <BasicTable @register="registerTable"
        @selection-change="onSelectChange"
        :clickToRowSelect="true"
        size="small">
        <template #toolbar>
          <!-- <a-button
            preIcon="ant-design:plus-circle-outlined"
            type="primary"
            @click="createCheckAll"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('创建年度盘点计划') }}
          </a-button> -->
          <a-button
            type="primary"
            @click="CreateCheck"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('创建盘点计划') }}
          </a-button>
          <a-button
            type="primary"
            @click="executeCheck"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('下达盘点计划') }}
          </a-button>
          <a-button
            type="primary"
            @click="cancelCheck"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('取消盘点计划') }}
          </a-button>
        </template>
        <template #isActive="{ record }">
          <Tag :color="record.isActive ? 'green' : 'red'">
            {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
          </Tag>
        </template>

      </BasicTable>
      <a-row style="height: 50%; margin-left: 15px; margin-right: 15px">
      <BasicTable @register="registerDetailTable"
       size="small">

      </BasicTable>
      </a-row>
      <CreateCheck
        @register="registercreateCheckModal"
        @reload="reload"
        :bodyStyle="{ 'padding-top': '0' }"
      />
    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent,ref } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { planColumns, searchFormSchema, getTableListAsync, planDetailColumns, Executing, getTableDetailListAsync,Delete } from './Check';
    import CreateCheck from './CreateCheck.vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    import { message } from 'ant-design-vue';
    import { useModal } from '/@/components/Modal';
    export default defineComponent({
      name: 'Check',
      components: {
        BasicTable,
        TableAction,
        CreateCheck,
        Tag,
      },
      setup() {
        const { createConfirm } = useMessage();
        const [registercreateCheckModal, { openModal: CreateCheck }] = useModal();
        const { t } = useI18n();
        let selectedBoxIdRef = ref(0);
        // table配置
        const [registerTable, { reload }] = useTable({
          columns: planColumns,
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
          maxHeight: 300,
        });

        const [registerDetailTable, { reload:reloadDetail}] = useTable({
          columns: planDetailColumns,
          api: getPageDetaiTableListAsync,
          showTableSetting: false,
          showIndexColumn: true,
          bordered: true,
          canResize: false,
          maxHeight: 300,
        });

        async function getPageDetaiTableListAsync(params) {
        if (selectedBoxIdRef.value == 0) {
          return {
            items: [],
            totalCount: 0,
          };
        }
        params.checkId = selectedBoxIdRef.value;
        return await getTableDetailListAsync(params);
      }
        //勾选事件
      const onSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedBoxIdRef.value = rows[0].id;
        } else {
          selectedBoxIdRef.value = 0;
        }
        reloadDetail();
      };
  
  
        //下达盘点计划
        async function executeCheck(){
          if(selectedBoxIdRef.value == 0){
            message.error("请先选择盘点计划")
            return
          }
          let msg = t('确认下达盘点计划？');
          let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await Executing(id);
              },
          })
        }
        //取消盘点计划
        async function cancelCheck(){
          if(selectedBoxIdRef.value == 0){
            message.error("请先选择盘点计划")
            return
          }
          let msg = t('确认取消盘点计划？');
          let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await Delete(id);
                reload();
              },
          })
        }
        return {
          registerTable,
          registerDetailTable,
          executeCheck,
          cancelCheck,
          t,
          reload,
          CreateCheck,
          onSelectChange,
          registercreateCheckModal,
        };
      },
    });
  </script>
  
