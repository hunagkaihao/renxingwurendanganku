<template>
  <BasicModal
    title="借用出库"
    :width="600"
    :canFullscreen="false"
    :destroyOnClose="true"
    :maskClosable="false"
    @ok="submit"
    @cancel="cancel"
    @register="registerModal"
  >
    <BasicForm @register="registerForm" />
  </BasicModal>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';
  import { BasicModal, useModalInner } from '/@/components/Modal';
  import { BasicForm, FormSchema, useForm } from '/@/components/Form';
  import { borrowStockOutAsync } from './Cell';

  const formSchema: FormSchema[] = [
    { field: 'cellCode', component: 'Input', label: '库位编码', componentProps: { disabled: true } },
    { field: 'materialCode', component: 'Input', label: '物料码', componentProps: { disabled: true } },
    { field: 'borrowPurpose', component: 'InputTextArea', label: '借用用途', required: true },
    {
      field: 'borrowDurationHours',
      component: 'InputNumber',
      label: '借用时长（小时）',
      required: true,
      componentProps: { min: 1, precision: 0 },
    },
  ];

  export default defineComponent({
    name: 'BorrowStockOut',
    components: { BasicModal, BasicForm },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerForm, { getFieldsValue, validate, setFieldsValue, resetFields }] = useForm({
        labelWidth: 120,
        schemas: formSchema,
        showActionButtonGroup: false,
      });
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner(async (data) => {
        await resetFields();
        await setFieldsValue({
          cellCode: data.record.cellCode,
          materialCode: data.record.materialCode,
          borrowPurpose: '',
          borrowDurationHours: 1,
        });
      });

      const submit = async () => {
        try {
          const values = getFieldsValue();
          await borrowStockOutAsync({
            request: {
              materialCode: values.materialCode,
              borrowPurpose: values.borrowPurpose,
              borrowDurationHours: values.borrowDurationHours,
            },
            changeOkLoading,
            validate,
            closeModal,
            resetFields,
          });
          emit('reload');
        } catch (_) {
          changeOkLoading(false);
        }
      };

      const cancel = () => {
        resetFields();
        closeModal();
      };

      return { registerModal, registerForm, submit, cancel };
    },
  });
</script>
