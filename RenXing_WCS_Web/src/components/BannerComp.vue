<template>
  <div>
    <el-row type="flex" align="start">
        <el-col :span="25">
            <el-dropdown @command="handleMenu">
                <el-button type="primary" size="small">
                    Menu<i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item command="toMainPage">主页</el-dropdown-item>
                    <el-dropdown-item command="toLogPage">日志</el-dropdown-item>
                </el-dropdown-menu>
            </el-dropdown>  
            <el-button v-if="wcsState === 'Running'" type="success" size="small" @click="pauseWcsServer">{{ wcsStateInChinese }}</el-button>
            <el-button v-else-if="wcsState === 'Pause'" type="warning" size="small" @click="restartWcsServer">{{ wcsStateInChinese }}</el-button>
            <el-button v-else size="small" type="info">{{ wcsStateInChinese }}</el-button>          
        </el-col>
    </el-row>
  </div>
</template>

<script>
import { Button, Dropdown, DropdownMenu, DropdownItem, Row, Col } from 'element-ui'    
import 'element-ui/lib/theme-chalk/index.css'

export default {
  data(){
    return {
      wcsState: 'unknown'
    }
  },
  computed: {
    wcsStateInChinese() {
        let _stateName = "";
        switch (this.wcsState) {
            case "Running":
                _stateName = "执行中";
                break;
            case "Pause":
                _stateName = "暂停中";
                break;
            default:
                _stateName = "未知状态";
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
    handleMenu(command){
        if(command === "toMainPage")
        {
            if(this.$route.path !== '/main/orderList' && this.$route.path !== '/main/oneOrder')
            {
                this.$router.push({
                    page:'/main/orderList'
                });
            }
        }
        else if(command === 'toLogPage')
        {
            if(this.$route.path !== '/log')
            {
                alert(command);
                this.$router.push({
                    page:'/log'
                });
            }
        }
    }
  },
  mounted() {
    this.orderData = [];
    this.timer = setInterval(() => {
        this.getWcsState();    
    }, 500);
  },
  beforeDestroy(){
    clearInterval(this.timer);
  },
  components: {
    'el-button': Button,
    'el-dropdown': Dropdown,
    'el-dropdown-menu': DropdownMenu,
    'el-dropdown-item': DropdownItem,
    'el-row': Row,
    'el-col': Col
  }
}
</script>

<style scoped>

</style>
