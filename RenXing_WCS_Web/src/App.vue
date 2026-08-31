<template>
  <div id="app"> 
    <el-row :gutter="20">
        <el-col :span="6">
            <div class="grid-content bg-purple">
                <el-dropdown trigger="click" @command="handleMenu">
                    <el-button type="primary" size="small" style="margin-right: 10px;">
                        <i class="el-icon-menu el-icon--right"></i>
                        Menu
                        <i class="el-icon-arrow-down el-icon--right"></i>
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                        <el-dropdown-item command="订单监控">订单监控</el-dropdown-item>
                        <el-dropdown-item command="点位监控">点位监控</el-dropdown-item>
                        <el-dropdown-item command="日志查询">日志查询</el-dropdown-item>
                    </el-dropdown-menu>
                </el-dropdown>
                <el-button v-if="wcsState === 'Running'" type="success" size="small" @click="pauseWcsServer">{{ wcsStateInChinese }}</el-button>
                <el-button v-else-if="wcsState === 'Pause'" type="warning" size="small" @click="restartWcsServer">{{ wcsStateInChinese }}</el-button>
                <el-button v-else size="small" type="info">{{ wcsStateInChinese }}</el-button>
            </div>
        </el-col>
        <el-col :span="12"><div class="grid-content bg-purple"></div></el-col>
        <el-col :span="6">
            <div class="grid-content bg-purple">
                <el-button type="info" size="small" @click="startTest">启动测试</el-button>
                <el-button type="info" size="small" @click="stopTest">停止测试</el-button>
                <el-button type="info" size="small" @click="restartTest">重启测试</el-button>
            </div>
        </el-col>
    </el-row>
    <router-view></router-view>
  </div>
</template>

<script>
import { Button, Row, Col, Dropdown, DropdownMenu, DropdownItem } from 'element-ui'    
import 'element-ui/lib/theme-chalk/index.css'
export default {
  name: 'App',
  data(){
    return {
        connection: null,
        wcsState: 'unknown',
        currentPage: ''
    }
  },
  computed: {
    wcsStateInChinese() {
        let _stateName = "";
        switch (this.wcsState) {
            case "Running":
                _stateName = "Wcs服务执行中";
                break;
            case "Pause":
                _stateName = "Wcs服务暂停中";
                break;
            default:
                _stateName = "Wcs服务状态未知";
        }
        return _stateName;
    }
  },
  methods:{
    pauseWcsServer()
    {
        this.$axios({
            method:'post',
            url:'wcs/dispatch/core/pause'
        }).then(res => {
            if(res.data)
            {
                if(res.data.success !== true)
                    alert('暂停wcs服务失败' + res.data.message);
            }
        }).catch(err => {
            alert(err);
        });
    
    },
    restartWcsServer()
    {
        this.$axios({
            method:'post',
            url:'wcs/dispatch/core/restart'
        }).then(res => {
            if(res.data)
            {
                if(res.data.success !== true)
                    alert('暂停wcs服务失败' + res.data.message);
            }
        }).catch(err => {
            alert(err);
        });
    },
    getWcsState(){
        let t = this;
        this.$axios({
            method:'get',
            url:'wcs/dispatch/core/wcsStatus'
        }).then(function(res){
            let jsonStr = JSON.stringify(res.data);
            t.wcsState = JSON.parse(jsonStr);
        }).catch(function(error){
            alert(error)
        });
    },
    startTest(){
        this.$axios({
            method:'post',
            url:'wcs/test/start'
        }).then(function(res){
            if(res.data.success == true)
                alert("服务器已接收启动测试指令");
            else
                alert("服务器接收启动测试失败");
        }).catch(function(error){
            alert(error)
        });
    },
    restartTest(){
        this.$axios({
            method:'post',
            url:'wcs/test/restart'
        }).then(function(res){
            if(res.data.success == true)
                alert("服务器已接收重启测试指令");
            else
                alert("服务器接收重启测试指令失败");
        }).catch(function(error){
            alert(error)
        });
    },
    stopTest(){
        this.$axios({
            method:'post',
            url:'wcs/test/stop'
        }).then(function(res){
            if(res.data.success == true)
                alert("服务器已接收停止测试指令");
            else
                alert("服务器接收停止测试指令失败");
        }).catch(function(error){
            alert(error)
        });
    },
    handleMenu(cmd){
        if(cmd === '订单监控')
        {
            this.currentPage = '订单监控';
            if(this.$route.path !== '/orderListMonitor')
            {
                this.$router.push({
                    path:'/orderListMonitor'
                });
            }
        }
        else if(cmd === '点位监控')
        {
            this.currentPage = '点位监控';
            if(this.$route.path !== '/tagMonitor')
            {
                this.$router.push({
                    path:'/tagMonitor'
                })
            }
        }
        else if(cmd === '日志查询')
        {
            this.currentPage = '日志查询';
            if(this.$route.path !== '/log')
            {
                this.$router.push({
                    path:'/log'
                });
            }
        }
    },
    updateWcsState(status)
    {
        this.wcsState = status;
        console.log(this.wcsState);
    }
  },
  mounted() {
    this.orderData = [];
    this.currentPage = '订单监控';
    this.getWcsState();
    this.$HubConn.on("UpdateWcsStatus", this.updateWcsState);
    if(this.$route.path !== '/orderListMonitor')
    {
      this.$router.push({
        path:'/orderListMonitor'
      });
    }
  },
  components: {
    'el-button': Button,
    'el-row': Row,
    'el-col': Col,
    'el-dropdown':Dropdown,
    'el-dropdown-menu':DropdownMenu,
    'el-dropdown-item':DropdownItem
  }
}
</script>

<style>
#app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    margin-top: 20px;
    margin-left: 10px;
    margin-right: 10px;
}
.bg-purple {
    background: transparent;
}
.grid-content {
    border-radius: 4px;
    min-height: 36px;
}
</style>
