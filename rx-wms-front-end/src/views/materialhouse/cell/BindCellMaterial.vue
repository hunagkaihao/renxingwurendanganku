<template>
  <BasicModal
    title="物料绑定"
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
  import { bindCellMaterialAsync, BindCellMaterialInput } from './Cell';

  const formSchema: FormSchema[] = [
    {
      field: 'cellCode',
      component: 'Input',
      label: '库位编码',
      componentProps: {
        disabled: true,
      },
    },
    {
      field: 'materialCode',
      component: 'Input',
      label: '物料码',
      required: true,
      componentProps: {
        autocomplete: 'off',
      },
    },
  ];

  export default defineComponent({
    name: 'BindCellMaterial',
    components: { BasicModal, BasicForm },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerForm, { getFieldsValue, validate, setFieldsValue, resetFields }] = useForm({
        labelWidth: 100,
        schemas: formSchema,
        showActionButtonGroup: false,
      });
      let selectedCellId = 0;

      const [registerModal, { changeOkLoading, closeModal }] = useModalInner(async (data) => {
        selectedCellId = data.record.id;
        await resetFields();
        await setFieldsValue({
          cellCode: data.record.cellCode,
          materialCode: '',
        });
      });

      const submit = async () => {
        try {
          const values = getFieldsValue();
          const request: BindCellMaterialInput = {
            cellId: selectedCellId,
            materialCode: values.materialCode,
          };
          await bindCellMaterialAsync({
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

      return { registerModal, registerForm, submit, cancel };
    },
  });
</script>
