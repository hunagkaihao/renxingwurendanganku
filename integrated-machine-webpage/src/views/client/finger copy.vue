
<template>
    <div>
        <Card dis-hover style="height:800px">
            <div class="page-body">
                <div class="margin-top-1">
                        <div class="main-header">
                            <!-- <div class="navicon-con"><Icon   custom="unarchive" size="32" color="#57a3f3" style="margin-top: 5px;"/></div> -->
                            <div class="navicon-con">
                                 <!-- <img class="xwblogo" src="../../../images/xwb1.png"  style="margin-top: 3px;"/> -->
                                 <!-- <img class="xwblogo" src="../../../images/qh.png"  style="margin-top: 3px;"/> -->
                                 <img class="xwblogo" :src="smallLogoUrl"  style="margin-top: 3px;"/>
                            <!-- <Icon   custom="unarchive" size="32" color="#57a3f3" style="margin-top: 5px;"/> -->
                            </div>
                             <div class="header-middle-con" style="margin-left: -25px;margin-top: -5px;"><h1>指纹识别</h1> </div>
                             <!-- <nav class="navbar-default" > <h1>取档</h1> </nav> -->
                            <div class="header-avator-con" style="margin-right: 10px;">     

                                <div class="user-dropdown-menu-con">
                                    <Row type="flex" justify="end" align="middle" class="user-dropdown-innercon" style="margin-left: -10px;">
                                       
                                           <Icon @click="logout" type="md-return-left" size="28" style="margin-left: 10px;margin-top: 5px;" color="LightSkyBlue" />

                                    </Row>
                                </div>

                            </div>
                        </div>

                </div>


             </div>  
                 
        </Card>
        
        
     </div>
</template>

<script>
import unlock from './unlock.vue';
import Cookies from 'js-cookie';
import userlogin from './userlogin.vue'
export default {
    components: {
        unlock,userlogin
    },
    data () {
        return {

        };
    },
    
    methods: {
        create(){
            this.createModalShow=true;
        },
        get smallLogoUrl(){
            return abp.setting.values.ClientSmallLogoPath
        },
        logout()
        {   
            this.$store.commit('app/logout', this);
            Util.abp.auth.clearToken();
            Cookies.set('facing', '0');
            Cookies.set('userId', '0');
            // location.reload();
            this.$router.push({
                name: 'login'
            });
        },

    handleUnlock () {
            this.$router.push({
                name: Cookies.get('last_page_name')
            });
      }
    },
    
    mounted () {
        this.showUnlock = true;
        if (!document.getElementById('lock_screen_back')) {
            let lockdiv = document.createElement('div');
            lockdiv.setAttribute('id', 'lock_screen_back');
            lockdiv.setAttribute('class', 'lock-screen-back');
            document.body.appendChild(lockdiv);
        }
        let lockScreenBack = document.getElementById('lock_screen_back');
        lockScreenBack.style.zIndex = -1;
    },
    veintest() {
    axios.get('http://localhost:5032/Vein/VeinMatch')
        .then(async (response) => {
          // this.serviceConnect=false;
          // console.log(response);
          if (response.data.result) {
            this.fingermge = "指纹测试成功" + response.data.result;
          } else {
            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }
          }
        })
        .catch((error) => {

            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }

        });
    },
};
</script>
<style scoped>
.page-body{
      height: 800px;  
}
.xwblogo{
    width: 32px;
    height: 32px;

}
.rect{
    width:30px;
    height: 90px;
    /* fill="#ebedf0" */

}
.rectC{
    width:70px;
    height: 138px;
    /* fill="#ebedf0" */

}
.rectsample{
    width:66px;
    height: 25px;
    /* fill="#ebedf0" */

}
.cabinet
{
    width:200px;
    height: 60px;
    stroke-width:1.5;
    stroke:#dcdee2;
    fill:none;
    /* fill="#ebedf0" */
}
.cabinet_box
{
       width:13px;
    height: 30px;
    stroke-width:1;
    stroke:#dcdee2;
    fill:none; 
}

.Nohave{
   fill:#f3f3f3;

}
.Selected{
   fill:#57a3f3;

}
.door
{
    text-align:center;
    height:248px;
    width:154px;
    margin-left: 40px;
    padding-top: 40px;
}
.DoorClose{
  background:#f3f3f3;

}
.DoorOHTask{

   background:#57a3f3;
}
.DoorOpen{
   background:#f02d37;

}
.DoorSelected{

   background:#2d8cf0;
}



.margin-top-20 {
    margin-top: 20px;
}
     .cardtitle{
    font-size: 16px;
    /* color: rgba(0,0,0,.85); */
    /* color:#e8eaec; */
    /* font-family: "Myriad Pro","Helvetica Neue",Arial,Helvetica,sans-serif; */
    font-weight: 600;
    font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB", "Microsoft YaHei", "\5FAE\8F6F\96C5\9ED1", Arial, sans-serif;
    line-height: 1.5;
    color: #515a6e;
    position: relative;
    top: 2px;
  }
  .margin-top-cm {
    margin-top: 50px;
}
  .margin-top-cm1 {
    margin-top: 82px;
}
  .taskstep
  {
      margin-left: 60px;
  }
    .doortitle
  {
      margin-bottom: 10px;
  }

</style>