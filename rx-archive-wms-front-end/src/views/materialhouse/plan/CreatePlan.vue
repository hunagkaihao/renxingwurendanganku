<template>
    <BasicModal
      :width="600"
      :title="t('创建计划任务')"
      :canFullscreen="false"
      @ok="submit"
      @cancel="cancel"
      @register="registerModal"
      @visible-change="visibleChange"
      :destroyOnClose="true"
      :maskClosable="false"
    >
      <BasicForm @register="registerCellForm" />
    </BasicModal>
    
  </template>
  
  <script lang="ts">
    import { defineComponent } from 'vue';
    import { BasicModal, useModalInner } from '/@/components/Modal';
    import { BasicForm, useForm } from '/@/components/Form/index';
    import {  createplanAsync , createPlanFormSchema } from './Plan';
    import { CellInitDto } from '/@/services/ServiceProxies';
    import { useI18n } from '/@/hooks/web/useI18n';
    export default defineComponent({
      name: 'CreatePlan',
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
          schemas: createPlanFormSchema,
          showActionButtonGroup: false,
        });
  
        const visibleChange = async (visible: boolean) => {
          if (visible) {
          } else {
            resetFields();
          }
        };
  
        // 保存用户
        const submit = async () => {
          try {
            let request = getFieldsValue() as CellInitDto;
            await createplanAsync({
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
  