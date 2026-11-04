<template>
  <BasicModal
    title="归还入库"
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
  import { returnStockInAsync } from './Cell';

  const formSchema: FormSchema[] = [
    { field: 'materialCode', component: 'Input', label: '物料码', required: true },
  ];

  export default defineComponent({
    name: 'ReturnStockIn',
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
        await setFieldsValue({ materialCode: data.record?.materialCode || '' });
      });

      const submit = async () => {
        try {
          const values = getFieldsValue();
          await returnStockInAsync({
            request: { materialCode: values.materialCode },
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
