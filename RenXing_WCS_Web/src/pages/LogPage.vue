<template>
    <div>
        <el-divider content-position="left"><font size="2" bold="true" face="Arial">日志查询</font></el-divider>
        <el-row :gutter="10" style="margin-bottom: 18px;">
            <el-col :span="3">
                <el-select v-model="select" @change="selectChanged" size="middle" placeholder="日志级别">
                    <el-option
                        v-for="item in options"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value">
                    </el-option>
                </el-select>
            </el-col>
            <el-col :span="7">
                <el-input v-model="input" clearable size="middle" placeHolder="消息片段，若不指定，请输入%"></el-input>
            </el-col>
            <el-col :span="3.5">
                <el-input-number v-model="maxCnt" controls-position="right" size="middle" placeholder="日志最大数量"></el-input-number>
            </el-col>
            <el-col :span="2">
                <el-button type="info" class="queryBtn" size="middle" @click="query">查询</el-button>
            </el-col>
        </el-row>
        <el-table
            :data="logData"
            size="mini"
            border
            stripe
            style="width: 100%;"
            :header-cell-style="{background:'#EBEEF5'}">
            <el-table-column prop="id" label="步骤Id" width="100px">
            </el-table-column>
            <el-table-column prop="date" label="时间" width="100px">
            </el-table-column>
            <el-table-column prop="grade" label="级别" width="80px">
            </el-table-column>
            <el-table-column prop="source" label="来源" width="200px">
            </el-table-column>
            <el-table-column prop="message" label="消息">
            </el-table-column>
        </el-table>       
    </div>
</template>

<script>
    import { Table, TableColumn, Button, Input, Select, Option, Row, Col, Divider, InputNumber } from 'element-ui'    
    import 'element-ui/lib/theme-chalk/index.css'
    export default{
        data(){
            return {
                input:'',
                select:'',
                maxCnt: 200,
                options:[
                    {
                        value:'INFO',
                        label:'INFO'
                    },
                    {
                        value:'ERROR',
                        label:'ERROR'
                    },
                    {
                        value:'WARN',
                        label:'WARN'
                    },
                    {
                        value:'FATAL',
                        label:'FATAL'
                    },
                    {
                        value:'DEBUG',
                        label:'DEBUG'
                    },
                    {
                        value:'ALL',
                        label:'ALL'
                    }
                ],
                logData:[
                    {
                        id:0,
                        date:'',
                        grade:'',
                        message:'',
                        source:''
                    }
                ]
            }
        },
        components:{
            'el-table': Table,
            'el-table-column': TableColumn,
            'el-button': Button,
            'el-input':Input,
            'el-select':Select,
            'el-option':Option,
            'el-row':Row,
            'el-col':Col,
            'el-divider':Divider,
            'el-input-number':InputNumber
        },
        methods:{
            selectChanged(value)
            {
                this.select=value;
            },
            query(){
                let t = this;

                let _grade = this.select;
                if(_grade === '')
                {
                    alert('请选择日志等级');
                    return;
                }
                if(_grade === 'ALL')
                    _grade = '%';
                
                let _msgSnap = this.input;
                if(_msgSnap === '')
                {
                    _msgSnap = '%';
                    this.input = '%';
                }
                let _maxLogCnt = this.maxCnt
                if(_maxLogCnt <= 0)
                {
                    alert("请指定有效的返回日志最大数量");
                    return;
                }

                this.$axios({
                    url:'ecs/log/query',
                    // url:'logquery/INFO/%/100',
                    method:'get',
                    params:{
                        msgSnip: _msgSnap,
                        grade: _grade,
                        logCntMax: _maxLogCnt
                    }
                    // url:'Face/FaceAdd',
                    // method:'post',
                    // data:{
                    //     UserId: "10",
                    //     ImageData: ""
                    // }
                }).then(res => {
                    if(res)
                    {
                        let strJson = JSON.stringify(res.data);
                        let obj = JSON.parse(strJson);
                        t.logData = obj;
                    }
                }).catch(err => {
                    console.log(err);
                });
            }
        },
        mounted(){
            this.logData = [];
        }
    }
</script>


<style>
.selectStyle {
    width: 150px;
}
.rowStyle {
    margin-top: 20px;
    margin-bottom: 0px;
}
</style>
  