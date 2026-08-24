<template>
    <div>       
        <el-divider content-position="left"><font size="2" bold="true" face="Arial">订单监控</font></el-divider>
        <el-table
            :data="orderData"
            size="mini"
            border
            stripe
            style="width: 100%"
            :header-cell-style="{background:'#EBEEF5'}">
            <el-table-column prop="orderCode" label="订单号" width="150">
            </el-table-column>
            <el-table-column prop="plateCode" label="档案盒" width="100">
            </el-table-column>
            <el-table-column prop="orderType" label="类型" width="100">
            </el-table-column>
            <el-table-column prop="startNode" label="起点" width="100">
            </el-table-column>
            <el-table-column prop="endNode" label="终点" width="100">
            </el-table-column>
            <el-table-column prop="execStep" label="当前步骤">
            </el-table-column>
            <el-table-column prop="orderState" label="状态" width="100">
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="240">
                <template slot-scope="scope">
                    <el-button type="info" size="mini" plain round @click="gotoOneOrderPage(scope.row)">查看</el-button>
                    <el-popconfirm title="确定要结束该任务吗？" 
                        confirm-button-text='是的'
                        cancel-button-text='不用了'
                        icon="el-icon-info"
                        icon-color="red"
                        @confirm="forceDoneOrder(scope.row)" 
                        style="margin-left: 10px; margin-right: 10px;">
                        <el-button type="warning" size="mini" plain round slot="reference">结束</el-button>
                    </el-popconfirm>
                    <el-popconfirm title="确定要取消该任务吗？" 
                        confirm-button-text='是的'
                        cancel-button-text='不用了'
                        icon="el-icon-info"
                        icon-color="red"
                        @confirm="cancelOrder(scope.row)">
                        <el-button type="danger" size="mini" plain round slot="reference">取消</el-button>
                    </el-popconfirm>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import { Button, Table, TableColumn, Divider, Popconfirm } from 'element-ui'    
    import 'element-ui/lib/theme-chalk/index.css'
    export default ({
    data() {
        return {
            orderData: [
                {
                    orderCode: "",
                    orderType: "",
                    orderState: "",
                    plateCode: "",
                    startNode: "",
                    endNode: "",
                    priority: 1,
                    createTime: "",
                    execStep: "",
                    execInfo: "",
                    hasError: false,
                    execUpdateTime: "",
                    pathId: 0,
                    taskId: 0,
                    taskState: ""
                }
            ]
        };
    },
    methods:{
        getOrderData(){
            let t = this;
            this.$axios({
                method:'get',
                url:'ecs/dispatch/order/unDoneOrders'
            }).then(function(res){
                let jsonStr = JSON.stringify(res.data);
                t.orderData = JSON.parse(jsonStr);
            }).catch(function(error){
                alert(error)
            });
        },
        updateOrderData(orders)
        {
            this.orderData = orders;
        },
        cancelOrder(row){
            this.$axios({
                method:'post',
                url:'ecs/dispatch/order/cancelOrder',
                data:{
                    orderCode: row.orderCode
                }
            }).then(res => {
                if(res.data)
                {
                    if(res.data.success === false)
                    {
                        alert(res.data.message);
                    }
                }
            }).catch(err => {
                alert(err);
            });
        },
        forceDoneOrder(row){
            this.$axios({
                method:'post',
                url:'ecs/dispatch/order/forceDone',
                data:{
                    orderCode: row.orderCode
                }
            }).then(res => {
                if(res.data)
                {
                    if(res.data.success === false)
                    {
                        alert(res.data.message);
                    }
                }
            }).catch(err => {
                alert(err);
            });
        },
        gotoOneOrderPage(row)
        {
            this.$router.replace({
                path:'/oneOrderMonitor',
                query:{
                    orderCode: row.orderCode
                }
            });
        }
    },
    mounted() {
        this.orderData = [];
        this.getOrderData(); 
        this.$HubConn.on("UpdateUndoneOrders", this.updateOrderData);
    },
    components:{
        'el-button': Button,
        'el-table': Table,
        'el-table-column': TableColumn,
        'el-divider': Divider,
        'el-popconfirm': Popconfirm
    }
})
</script>

<style>
    
</style>
