<template>
    <div>  
        <el-row :gutter="30">
            <el-col :span="14">
                <el-divider content-position="center"><font size="2" bold="true" face="Arial">PLC点位</font></el-divider>
            </el-col>                
            <el-col :span="10">
                <el-divider content-position="center"><font size="2" bold="true" face="Arial">密集架点位</font></el-divider>
            </el-col>
        </el-row>

        <el-row :gutter="30">
            <el-col :span="14">
                <el-table
                    :data="plcTagData"
                    size="mini"
                    border
                    stripe
                    style="width: 100%"
                    :header-cell-style="{background:'#EBEEF5'}">
                    <el-table-column prop="monitorTagName" label="变量名称" align="center" width="150"></el-table-column>
                    <el-table-column prop="monitorTagAddr" label="变量地址" align="center" width="150"></el-table-column>
                    <el-table-column prop="monitorTagValue" label="变量值" align="center"></el-table-column>
                </el-table> 
            </el-col>                
            <el-col :span="10">
                <el-table
                    :data="mjjData"
                    size="mini"
                    border
                    stripe
                    style="width: 100%"
                    :header-cell-style="{background:'#EBEEF5'}">
                    <el-table-column prop="tagName" label="变量名称" align="center" width="200"></el-table-column>
                    <el-table-column prop="tagValue" label="变量值" align="center"></el-table-column>
                </el-table>
            </el-col>
        </el-row>      
    </div>
</template>

<script>
    import { Table, TableColumn, Divider, Row, Col } from 'element-ui'    
    import 'element-ui/lib/theme-chalk/index.css'
    export default ({
    data() {
        return {
            plcTagData: [
                {
                    monitorTagName: "",
                    monitorTagAddr: "",
                    monitorTagValue: ""
                }
            ],
            mjjData: [
                {
                    tagName: "",
                    tagValue: ""
                }
            ]
        };
    },
    methods:{
        getPlcTagData(){
            let t = this;
            this.$axios({
                method:'get',
                url:'wcs/plc/plcMonitor'
            }).then(function(res){
                let jsonStr = JSON.stringify(res.data);                
                t.plcTagData = JSON.parse(jsonStr);
            }).catch(function(error){
                alert(error)
            });
        },
        updatePlcTagData(plcTags)
        {
            this.plcTagData = plcTags;
        },
        getMjjTagData(){
            let t = this;
            this.$axios({
                method:'get',
                url:'wcs/mjj/mjjStatusOfNmValMapList'
            }).then(function(res){
                let jsonStr = JSON.stringify(res.data);       
                t.mjjData = JSON.parse(jsonStr);     
            }).catch(function(error){
                alert(error)
            });
        },
        updateMjjTagData(data)
        {
            this.mjjData = data;
        }
    },
    mounted() {
        this.plcTagData = [];
        this.mjjData = [];
        this.getPlcTagData(); 
        this.getMjjTagData();
        this.$HubConn.on("UpdatePlcTags", this.updatePlcTagData);
        this.$HubConn.on("UpdateMjjStatus", this.updateMjjTagData);
    },
    components:{
        'el-table': Table,
        'el-table-column': TableColumn,
        'el-divider': Divider,
        'el-row': Row,
        'el-col': Col
    }
})
</script>

<style>
    
</style>
