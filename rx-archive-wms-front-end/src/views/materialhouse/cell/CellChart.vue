<template>
  <div>
    <a-card>
      <a-form>
        <a-row :gutter="24">
          <a-col :span="12">
            <a-form-item
              :label="t('选择货架') + ':'"
              :label-col="{ span: 6, offset: 0 }"
              :wrapper-col="{ span: 12, offset: 0 }"
            >
              <a-select
                :placeholder="t('Select')"
                clearable
                ref="select"
                v-model:value="rowcn"
                @change="handleChange"
              >
                <a-select-option v-for="n in maxZ" :key="n" :value="n">第{{ n }}排</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item
              :label="t('库位') + ':'"
              :label-col="{ span: 6, offset: 0 }"
              :wrapper-col="{ span: 12, offset: 0 }"
            >
              <a-input disabled="disabled" v-model:value="cellCode" :placeholder="t('所选库位')" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
      <!-- <BasicForm @register="registerCellChartForm" /> -->
      <row>
        <a-button type="primary" size="small" @click="lock">{{ t('锁定') }}</a-button>
        <a-button type="primary" size="small" @click="unlock">{{ t('解锁') }}</a-button>
        <p></p>
      </row>
      <div class="margin-top-10">
        <Row :gutter="16">
          <svg width="828" height="40" class="js-calendar-graph-svg">
            <rect class="rect Full" x="200" y="10" />
            <text dx="230" dy="25">有货货位</text>
            <rect class="rect Nohave" x="350" y="10" />
            <text dx="380" dy="25">无货货位</text>
            <rect class="rect Selected" x="500" y="10" />
            <text dx="530" dy="25">工作中货位</text>
            <rect class="rect Disable" x="650" y="10" />
            <text dx="680" dy="25">禁用货位</text>
          </svg>
        </Row>
        <Row :gutter="24">
          <svg width="1000" height="228" class="js-calendar-graph-svg">
            <g transform="translate(10, 20)">
              <g v-for="n in maxX" :key="'x_' + n" :transform="'translate(' + n * 22 + ', 0)'">
                <rect
                  @click="showBTT(item.cellCode, item.id)"
                  v-for="(item, i) in cellList
                    .filter((a) => a.cell_x == n && a.cell_z == rowcn)
                    .reverse()"
                  :key="item.id"
                  :class="'rect ' + item.cellStatus + ' ' + item.runStatus"
                  :x="n"
                  :y="i * 34"
                  :data-date="item.id"
                />
              </g>
            </g>
            <text v-for="n in maxX" :key="'x1_' + n" :x="n * 23 + 13" y="12" class="month">
              {{ n }}
            </text>
            <!-- <text x="71" y="12" class="month">2</text> -->
            <text v-for="n in maxY" :key="'y_' + n" text-anchor="start" dx="15" :dy="n * 34 + 5">
              {{ maxY + 1 - n }}
            </text>
            <!-- <text text-anchor="start" class="wday" dx="15" dy="61">2</text> -->
          </svg>
        </Row>
      </div>
    </a-card>
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref } from 'vue';
  import { Button } from '/@/components/Button';
  import { BasicForm } from '/@/components/Form/index';
  import { getTableListByZAsync } from './Cell';
  import { Form } from 'ant-design-vue';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { CellDto, PagingCellListInput } from '/@/services/ServiceProxies';
  export default defineComponent({
    name: 'CellChart',
    components: {
      Button,
      BasicForm,
      Form,
    },
    setup() {
      console.log('setup');
      const { t } = useI18n();
      // const [registerCellChartForm, { getFieldsValue, setFieldsValue, resetFields, updateSchema }] =
      //   useForm({
      //     labelWidth: 120,
      //     schemas: cellChartFormSchema,
      //     showActionButtonGroup: false,
      //   });
      const rowcn = ref(1);
      let maxZ = 2;
      let maxX = 2;
      let maxY = 2;
      let cellId = 0;
      let cellCode = ref('');
      class MyCell {
        id: number | undefined;
        cellCode: string | undefined;
        cell_x: number | undefined;
        cell_y: number | undefined;
        cell_z: number | undefined;
        cellStatus: string | undefined;
        runStatus: string | undefined;
      }
      // let cellList: MyCell[] = [];

      const cellList = ref([
        // {
        //   id: 1,
        //   cellCode: '01-01-01',
        //   cell_x: 1,
        //   cell_y: 1,
        //   cell_z: 1,
        //   cellStatus: 'Have',
        //   runStatus: 'Enable',
        // } as MyCell,
      ]);
      getCells(1); //初始化

      // class MySelectItem {
      //   label: string | undefined;
      //   value: string | undefined;
      // }
      // {
      //   let itemCells: MySelectItem[] = [];
      //   for (let index = 1; index <= rowcn.value; index++) {
      //     let cellitem = new MySelectItem();
      //     cellitem.label = '第' + index + '排';
      //     cellitem.value = index.toString();
      //     itemCells.push(cellitem);
      //   }

      //   console.log(itemCells);
      //   (cellChartFormSchema[0].componentProps as any).options = itemCells;
      //   updateSchema(cellChartFormSchema); //更新架构数据
      // }
      function isSelectChange() {
        alert(1);
      }

      async function getCells(cellZ: number) {
        let params: PagingCellListInput = new PagingCellListInput();
        params.cellZ = parseInt(cellZ);
        // cellList = [];
        await getTableListByZAsync(params)
          .then((response) => {
            // console.log(response);
            // let itemCells: CellItrem[] = [];
            cellList.value = response.items as MyCell[];
          })
          .catch(() => {
            // message.error(t('common.operationFail'));
          });
      }
      const handleChange = async (value: string) => {
        // alert(`selected ${value}`);
        // let myCell = {
        //   id: 1,
        //   cellCode: '02-01-01',
        //   cell_x: 1,
        //   cell_y: 1,
        //   cell_z: 2,
        //   cellStatus: 'Nohave',
        //   runStatus: 'Disable',
        // };
        // cellList.push(myCell);
        let params: PagingCellListInput = new PagingCellListInput();
        params.cellZ = parseInt(value);
        // cellList = [];
        await getTableListByZAsync(params)
          .then((response) => {
            // console.log(response);
            // let itemCells: CellItrem[] = [];
            cellList.value = response.items as MyCell[];
            // cellList.splice(0);
            // response.items?.forEach((e) => {
            //   let myCell = new MyCell();
            //   myCell.id = e.id;
            //   myCell.cellCode = e.cellCode;
            //   myCell.cell_x = e.cell_x;
            //   myCell.cell_y = e.cell_y;
            //   myCell.cell_z = e.cell_z;
            //   myCell.cellStatus = e.cellStatus;
            //   myCell.runStatus = e.runStatus;
            //   cellList.push(myCell);
            //   // itemCells.push(cellitem);
            // });
          })
          .catch(() => {
            // message.error(t('common.operationFail'));
          });
        // var getcellLists = getLists.items as CellDto[];

        // await cellList.splice(0);
        // console.log(getcellLists);
        // for (let index = 0; index < getcellLists.length; index++) {
        //   let myCell = new MyCell();
        //   myCell.id = getcellLists[index].id;
        //   myCell.cellCode = getcellLists[index].cellCode;
        //   myCell.cell_x = getcellLists[index].cell_x;
        //   myCell.cell_y = getcellLists[index].cell_y;
        //   myCell.cell_z = getcellLists[index].cell_z;
        //   myCell.cellStatus = getcellLists[index].cellStatus;
        //   myCell.runStatus = getcellLists[index].runStatus;
        //   cellList.push(myCell);
        // }
        // for (let index = 0; index < 1; index++) {
        //   let myCell = {
        //     id: 1,
        //     cellCode: '02-01-01',
        //     cell_x: 1,
        //     cell_y: 1,
        //     cell_z: 2,
        //     cellStatus: 'Nohave',
        //     runStatus: 'Disable',
        //   };
        //   cellList.push(myCell);
        // }
        // console.log(cellList);
        // console.log(rowcn.value);
        // console.log(cellList.filter((a) => a.cell_x == 1 && a.cell_z == rowcn.value).reverse());
      };

      function lock() {}
      function unlock() {}

      function showBTT(val: string, vid: number) {
        console.log(1);
        if (cellId != 0 && cellId == vid) {
          cellId = 0;
          cellCode.value = '';
        } else {
          // alert(val);
          cellId = vid;
          cellCode.value = val;
          // setFieldsValue({
          //   cellCode: val,
          // });
        }
      }

      return {
        rowcn,
        maxZ,
        maxX,
        maxY,
        cellCode,
        isSelectChange,
        handleChange,
        lock,
        unlock,
        showBTT,
        cellList,
        t,
      };
    },
    async created() {
      console.log('created');
    },
    beforeCreate() {
      console.log('beforeCreate');
    },
  });
</script>
<style scoped>
  .rect {
    width: 15px;
    height: 30px;
    /* fill="#ebedf0" */
  }
  .Full {
    fill: aqua;
    /* fill="#ebedf0" */
  }
  .Have {
    fill: aqua;
    /* fill="#ebedf0" */
  }
  .Nohave {
    fill: lightgray;
  }
  .Selected {
    fill: orange;
  }
  .Disable {
    fill: lightcoral;
  }
  svg {
    margin-left: 10px;
  }
</style>
