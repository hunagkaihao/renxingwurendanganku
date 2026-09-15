<template>
  <BasicModal :width="700" title="创建物料入库预约" :canFullscreen="false" :destroyOnClose="true" :maskClosable="false" @register="registerModal" @ok="submit" @cancel="cancel">
    <BasicForm @register="registerForm" />
  </BasicModal>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';
  import moment from 'moment';
  import { BasicModal, useModalInner } from '/@/components/Modal';
  import { BasicForm, useForm } from '/@/components/Form';
  import { CreateStockTaskDto } from '/@/services/ServiceProxies';
  import { createFormSchema, createWCSInAsync } from './StockTask';
  import { buildCellInboundDefaults } from '../cell/CellStockAction';

  export default defineComponent({
    name: 'CreateStockTask',
    components: { BasicModal, BasicForm },
    emits: ['reload'],
    setup(_, { emit }) {
      const [registerForm, { getFieldsValue, validate, resetFields, setFieldsValue }] = useForm({
        labelWidth: 120,
        schemas: createFormSchema,
        showActionButtonGroup: false,
      });
      const [registerModal, { changeOkLoading, closeModal }] = useModalInner(async (data) => {
        await resetFields();
        await setFieldsValue({
          materialCreateTime: moment().format('YYYY-MM-DD HH:mm:ss'),
          ...(data?.record ? buildCellInboundDefaults(data.record) : {}),
        });
      });
      const submit = async () => {
        try {
          await createWCSInAsync({ request: getFieldsValue() as CreateStockTaskDto, changeOkLoading, validate, closeModal, resetFields });
          emit('reload');
        } catch {
          changeOkLoading(false);
        }
      };
      const cancel = () => { resetFields(); closeModal(); };
      return { registerModal, registerForm, submit, cancel };
    },
  });
</script>
