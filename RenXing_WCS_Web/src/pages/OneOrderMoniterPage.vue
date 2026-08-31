<template>
    <div>
        <el-divider content-position="left"><font size="2" bold="true" face="Arial">订单监控</font></el-divider>
        <p align="left">
            <el-button size="small" type="info" style="margin-right: 20px;" @click="gotoOrderListPage">
                <i class="el-icon-arrow-left el-icon--left"></i>返回
            </el-button>
            <el-tag type="success">订单：{{ orderCode }}</el-tag>
            <el-tag >任务Id：{{ taskId }}</el-tag>
            <el-tag type="info">类型：{{ orderType }}</el-tag>
            <el-tag type="warning">起止点：{{ path }}</el-tag>            
        </p>
        <el-table
            :data="steps"
            border
            size="mini"
            stripe
            style="width: 100%"
            :header-cell-style="{background:'#EBEEF5'}">
            <el-table-column prop="id" label="步骤Id" width="100">
            </el-table-column>
            <el-table-column prop="pathStep" label="步骤号" width="100">
            </el-table-column>
            <el-table-column prop="nodeName" label="执行节点" width="170">
            </el-table-column>
            <el-table-column prop="cmdName" label="命令" width="170">
            </el-table-column>
            <el-table-column prop="state" label="状态" width="100">
            </el-table-column>
            <el-table-column prop="execInfo" label="执行信息">
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import { Table, TableColumn, Tag, Button, Divider } from 'element-ui'    
    import 'element-ui/lib/theme-chalk/index.css'
    export default{
        data(){
            return {
                hubConnection: null,
                orderCode: "",
                orderType: "",
                taskId: 0,
                path: "",
                steps:[{
                    id: 0,
                    pathStep: 0,
                    nodeName: "",
                    cmdName: "",
                    state: "",
                    execInfo: ""
                }]
            }
        },
        components:{
            'el-table': Table,
            'el-table-column': TableColumn,
            'el-tag': Tag,
            'el-button': Button,
            'el-divider': Divider
        },
        methods:{
            updateOrder()
            {
                var t = this;
                this.$axios({
                    method: 'get',
                    url: 'wcs/dispatch/order/oneOrder',
                    params:
                    {
                        orderCode: this.$route.query.orderCode
                    }
                }).then(function(res){
                    if(res)
                    {
                        let jsonStr = JSON.stringify(res.data);
                        let obj = JSON.parse(jsonStr);
                        t.orderCode = obj.orderCode;
                        t.orderType = obj.orderType;
                        t.taskId = obj.taskId
                        t.path = obj.startNode + "  to  " + obj.endNode;
                        t.steps = obj.jobs;
                        //console.log(t.steps);
                    }
                }).catch(function(err){ 
                    alert(err); 
                });
            },
            updateOneOrder(undoneOrders)
            {
                // console.log(undoneOrders)
                let orderCode = this.$route.query.orderCode;
                let order = null;
                for(let od of undoneOrders)
                {
                    if(od.orderCode == orderCode)
                    {
                        order = od;
                        break;
                    }
                }
                //console.log(undoneOrders)
                if(order == null)
                {
                    this.steps = [];
                    return;
                }

                this.orderCode = order.orderCode;
                this.orderType = order.orderType;
                this.taskId = order.taskId
                this.path = order.startNode + "  to  " + order.endNode;
                this.steps = order.jobs;
                //console.log(this.steps);   
            },
            gotoOrderListPage()
            {
                this.$router.replace({
                    path:'/orderListMonitor'
                });
            }
        },
        mounted()
        {
            this.orders = [];
            this.steps = [];
            this.updateOrder();
            this.$HubConn.on("UpdateUndoneOrders", this.updateOneOrder);
        }
    }
</script>


<style>
.el-tag{
    margin-right: 5px;
}
.returnBtn{
    margin-left: 15px;
}

</style>
