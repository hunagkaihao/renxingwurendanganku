<template>
    <BasicModal
      :width="600"
      :title="t('创建盘点计划')"
      :canFullscreen="false"
      @ok="submit"
      @cancel="cancel"
      @register="registerModal"
      @visible-change="visibleChange"
      :destroyOnClose="true"
      :maskClosable="false"
    >
      <p>{{ t('填写 0 表示该方向全部库位，默认三个坐标均为 0，表示全库盘点。') }}</p>
      <p>{{ t('例如：排号 1、列号 0、层号 0 表示第 1 排全部库位；排号 0、列号 0、层号 2 表示全部排的第 2 层。') }}</p>
      <p>{{ t('创建后仅生成待下达计划，点击“下达盘点计划”后才会执行。') }}</p>
      <BasicForm @register="registerCellForm" />
    </BasicModal>
    
  </template>
  
  <script lang="ts">
    import { defineComponent } from 'vue';
    import { BasicModal, useModalInner } from '/@/components/Modal';
    import { BasicForm, useForm } from '/@/components/Form/index';
    import {  createCheckBatAsync , createCheckBatFormSchema } from './Check';
    import { useI18n } from '/@/hooks/web/useI18n';
    export default defineComponent({
      name: 'CreateCkeck',
      components: {
        BasicModal,
        BasicForm,
      },
      emits: ['reload'],
      setup(_, { emit }) {
        // 加载父组件方法
        // defineEmits(['reload']);
        // const ctx = useContexts();
  
        const { t } = useI18n();
  
        const [registerModal, { changeOkLoading, closeModal }] = useModalInner();
        const [registerCellForm, { getFieldsValue, validate, resetFields }] = useForm({
          labelWidth: 120,
          schemas: createCheckBatFormSchema,
          showActionButtonGroup: false,
        });
  
        const visibleChange = async (visible: boolean) => {
          if (visible) {
          } else {
            resetFields();
          }
        };
  
        // 保存盘点范围；创建计划不会立即下发设备任务。
        const submit = async () => {
          try {
            const request = getFieldsValue();
            await createCheckBatAsync({
              request,
              changeOkLoading,
              validate,
              closeModal,
              resetFields,
            });
            emit('reload');
          } catch (error) {
            changeOkLoading(false);
          }
        };
        const cancel = () => {
          resetFields();
          closeModal();
        };
        return {
          t,
          cancel,
          registerModal,
          registerCellForm,
          submit,
          visibleChange,
        };
      },
    });
  </script>
  <style lang="less" scoped>
    .ant-checkbox-wrapper + .ant-checkbox-wrapper {
      margin-left: 0px;
    }
  </style>
