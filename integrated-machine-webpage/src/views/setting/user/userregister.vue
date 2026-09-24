<style lang="less">
@import "../../head.less";
</style>
<template>
  <div>
    <Card dis-hover style="height: 800px" >
      <div class="page-body">
        <div class="margin-top-1">
          <div class="main-header">
            <div class="navicon-con">
              <Icon
                type="md-person-add"
                size="32"
                color="#57a3f3"
                style="margin-top: 5px"
              />
            </div>
            <div
              class="header-middle-con"
              style="margin-left: -30px; margin-top: -5px"
            >
              <h1>用户数据注册</h1>
            </div>
            <div class="header-avator-con" style="margin-right: 10px">
              <div class="user-dropdown-menu-con">
                <Row
                  type="flex"
                  justify="end"
                  align="middle"
                  class="user-dropdown-innercon"
                >
                  <Dropdown transfer trigger="click">
                    <a href="javascript:void(0)">
                      <span class="main-user-name">{{ userName }}</span>
                      <Icon type="arrow-down-b"></Icon>
                    </a>
                  </Dropdown>
                  <!-- <span
                    class="avatar"
                    style="background: #619fe7; margin-left: 10px"
                    ><img src="../../../images/usericon.jpg"
                  /></span> -->
                  <Icon
                    @click="logout"
                    type="md-return-left"
                    size="28"
                    style="margin-left: 10px; margin-top: 5px"
                    color="LightSkyBlue"
                  />
                </Row>
              </div>
            </div>
          </div>
        </div>
        <Row :gutter="24">
          <Col span="8">
            <div class="margin-top-10">
              <h4>用户查询</h4>
            </div>
            <!-- <Form ref="queryForm" :label-width="70" label-position="left" inline> -->
            <div class="margin-top-10">
              <!-- <Input v-model="pagerequest.keyword"  suffix="ios-search" :placeholder="L('UserName')+'/'+L('Name')"></Input> -->
              <Input v-model="pagerequest.keyword" placeholder="输入关键字" @keyup.enter.native="getpage2">
               <Icon type="ios-keypad" slot="suffix"  @click="openKeyboard" />
                <Icon type="ios-search" slot="suffix" @click="getpage2" />
              </Input>
            </div>
            <!-- </Form> -->
            <div class="margin-top-10">
              <Table
                :loading="loading"
                :columns="columns"
                :no-data-text="L('NoDatas')"
                border
                highlight-row
                ref="selection"
                :data="list"
                @on-row-click="rowselect"
              >
              </Table>
              <Page
                show-sizer
                class-name="fengpage"
                :total="totalCount"
                class="margin-top-10"
                @on-change="pageChange"
                @on-page-size-change="pagesizeChange"
                :page-size="8"
                :page-size-opts="[8]" 
                :current="currentPage"
              ></Page>
            </div>
            <div class="margin-top-10 buttondiv2">
              <Button
                @click="create"
                icon="android-add"
                type="primary"
                size="large"
                >{{ L("新增") }}</Button>
               <Button
                @click="edituser"
                icon="android-add"
                class="toolbar-btn"
                type="primary"
                size="large"
                >{{ L("编辑") }}</Button>
            </div>
          </Col>
          <Col span="16">
            <div class="demo-split">
              <Split v-model="0.53" mode="vertical">
                <div slot="top" class="demo-split-pane">
                  <div class="margin-top-10">
                    <h4>人脸数据注册</h4>
                  </div>
                  <Row>
                    <Col span="10">
                      <div class="margin-top-10 imgdiv">
                        <!-- <img ref="myimg"  id="myimg" src="../../../images/emptyImg.png"> -->
                        <img ref="myimg" id="myimg" :src="faceUser.facePath" />
                      </div>
                      <div class="margin-top-10" style="text-align: center">
                        <h5>所选择用户 {{ userNameSelect }}</h5>
                      </div>
                    </Col>
                    <Col span="14">
                      <Row>
                        <Col span="3"> </Col>
                        <Col span="18">
                          <div class="margin-top-10 videodiv">
                            <div class="topvideoleft"></div>
                            <div class="topvideoright"></div>
                            <div
                              v-show="canvasShow"
                              id="videomask"
                              class="videomask"
                            ></div>
                            <canvas
                              id="canvas"
                              width="336px"
                              height="189px"
                            ></canvas>
                            <video
                              v-show="!canvasShow"
                              id="video"
                              class="unlock-avator-img"
                            >
                              您的浏览器不支持 video 标签
                            </video>
                            <!-- <canvas hidden="hidden" id="canvas" width="336px" height="189px"></canvas> -->
                          </div>
                          <div class="margin-top-10" style="text-align: center">
                            <h5>{{ mge }}</h5>
                          </div>
                          <div class="margin-top-10 buttondiv1">
                            <Button
                              @click="register"
                              icon="android-add"
                              type="primary"
                              size="large"
                              >{{ L("注册") }}</Button
                            >
                            <Button
                              type="primary"
                              size="large"
                              @click="faceDelete"
                              class="toolbar-btn"
                              >{{ L("删除") }}</Button
                            >
                            <Button
                              type="primary"
                              size="large"
                              @click="faceTest"
                              class="toolbar-btn"
                              >{{ L("测试") }}</Button
                            >
                          </div>
                        </Col>
                        <Col span="3"> </Col>
                      </Row>
                    </Col>
                  </Row>
                </div>
                <div slot="bottom" class="demo-split-pane">
                  <div class="margin-top-10">
                    <h4>指静脉数据注册</h4>
                  </div>
                  <Row>
                    <Col span="6">
                      <div class="margin-top-10"></div>
                    </Col>
                    <Col span="12">
                      <div class="margin-top-10">
                        <Table
                          :columns="columns2"
                          :no-data-text="L('NoDatas')"
                          border
                          highlight-row
                          ref="selection2"
                          :data="list2"
                          @on-row-click="rowselect2"
                        >
                        </Table>
                      </div>
                      <h5>{{ fingermge }}</h5>
                      <div class="margin-top-10 buttondiv2">
                        <Button
                          @click="veinregister"
                          icon="android-add"
                          type="primary"
                          size="large"
                          >{{ L("注册") }}</Button
                        >
                        <Button
                          type="primary"
                          size="large"
                          @click="veindelete"
                          class="toolbar-btn"
                          >{{ L("删除") }}</Button
                        >
                        <Button
                          type="primary"
                          size="large"
                          @click="veintest"
                          class="toolbar-btn"
                          >{{ L("测试") }}</Button
                        >
                      </div>
                    </Col>
                    <Col span="6"> </Col>
                  </Row>
                </div>
              </Split>
            </div>
          </Col>
        </Row>
      </div>
    </Card>
    <create-user
      v-model="createModalShow"
      @save-success="getpage"
    ></create-user>
    <edit-user v-model="editModalShow" @save-success="getpage"></edit-user>
    <face-user v-model="editfaceModalShow" @save-success="getpage"></face-user>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import CreateUser from "./create-user.vue";
import EditUser from "./edit-user.vue";
import FaceUser from "./face-user.vue";
import appconst from "../../../lib/appconst";
import axios from "axios";
import Cookies from "js-cookie";
import User from "@/store/entities/user";
class PageUserRequest extends PageRequest {
  keyword: string;
  isActive: boolean = null; //nullable
  from: Date;
  to: Date;
}
function changeTitle() {
  console.log(3333);
  alert(333);
}
@Component({
  components: { CreateUser, EditUser, FaceUser },
})
export default class Users extends AbpBase {
  edit() {
    this.editModalShow = true;
  }
  editface() {
    this.editfaceModalShow = true;
  }
  //filters
  pagerequest: PageUserRequest = new PageUserRequest();
  creationTime: Date[] = [];

  createModalShow: boolean = false;
  editModalShow: boolean = false;
  editfaceModalShow: boolean = false;

  imgName: string = "";
  visible: boolean = false;
  uploadList: any = [];
  file: any = null;
  imgUrl: any = "";
  mge: String = "人脸注册";
  fingermge: String = " ";
  get list() {
    return this.$store.state.user.list;
  }
  get list2() {
    return this.$store.state.user.userFingerVeins;
  }
  get loading() {
    return this.$store.state.user.loading;
  }
  get userName() {
    return this.$store.state.session.user
      ? this.$store.state.session.user.name
      : "";
  }
  get faceUser() {
    return this.$store.state.user.faceUser;
  }
  create() {
    this.createModalShow = true;
  }
edituser()
{     
  if (this.userId == 0) {
      this.$Message.error("请先选择用户");
      return;
    }
    this.$store.commit('user/edit',this.userSelect);
      this.editModalShow=true;
      //  this.edit(); 
}
//   logout() {
//     // this.$store.commit('app/logout', this);
//     // Util.abp.auth.clearToken();
//     // Cookies.set('facing', '0');
//     // Cookies.set('userId', '0');
//     //location.reload();
//     //  this.$router.push({
//     //     name: 'login'
//     // });
//     this.$router.push({
//       name: "client",
//     });
//   }
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
           // location.reload();
        }

  isActiveChange(val: string) {
    console.log(val);
    if (val === "Actived") {
      this.pagerequest.isActive = true;
    } else if (val === "NoActive") {
      this.pagerequest.isActive = false;
    } else {
      this.pagerequest.isActive = null;
    }
  }
  pageChange(page: number) {
    this.$store.commit("user/setCurrentPage", page);
    this.getpage();
  }
  pagesizeChange(pagesize: number) {
    this.$store.commit("user/setPageSize", pagesize);
    this.getpage();
  }
  userId: number = 0;
  veinId: number = 0;
  Faceflag:true;
  userNameSelect: string = "";
  userSelect:User=new User();
  empry_url: string = "../../assets/images/emptyImg.png";
  rowselect(currentRow, index) {
    this.userId = currentRow.id;
    this.veinId = 0;
    this.userNameSelect = currentRow.userName;
    this.userSelect=currentRow as User;
    this.fingermge= "";
    // let img=document.getElementById("myimg");
    this.$store.dispatch({
      type: "user/getUrl",
      id: currentRow.id,
    });
    // if(currentRow.facePath==null)
    // {
    //     (this.$refs.myimg as any).src=null;
    // }
    // else
    // {
    //      (this.$refs.myimg as any).src=currentRow.facePath;
    // }
    this.$store.dispatch({
      type: "user/getVeins",
      userId: currentRow.id,
    });
    //facePath
    //alert(currentRow.facePath);
  }
  rowselect2(currentRow, index) {
    this.veinId = currentRow.id;
  }
  registerFlag: boolean = false;
  async register() {
    if (this.userId == 0) {
      alert("请先选择用户");
      return;
    }
    console.log(this.faceUser.facePath)
    if (this.faceUser.facePath !=null) {
      alert("系统中已存在该用户的人脸数据");
      return;
    }
    this.canvasShow = true;
    this.faceTestFlag = false;
    this.registerFlag = false;
    this.mge = "开始人脸注册";
    this.getMedia2();
    let times = 20;
    console.time("runTime:");
    for (let index = 0; index < times; index++) {
      //console.log(index.toString() + new Date());
      // setTimeout(this.validatorphoto,1000);
      if (index > 0) {
        this.canvasShow = false;
      }
      await this.sleep(350).then(async () => {
        // 这里写sleep之后需要去做的事情
        console.log(index.toString() + new Date());
        console.time("test" + index);
      });
      if (index > 0 ) {
        await this.faceregister(index)
          .then(() => {
            this.registerFlag = true;
            this.$store.dispatch({
            type: "user/getUrl",
            id: this.userId,
            });
          })
          .catch(function (error) {
            console.log(error);
            //return false;
          });
      }
      if (this.registerFlag) {
        console.log(index)
        index = 9999;
        console.timeEnd("runTime:");
        //  this.closeMedia();
        return;
      }
    }
    console.timeEnd("runTime:");
    this.mge = "人脸注册失败";
    this.closeMedia();

  }
  faceTestFlag: boolean = false;
  canvasShow: boolean = false;
  async faceTest() {
    if (this.userId == 0) {
      alert("请先选择用户");
      return;
    }
    this.canvasShow = true;
    this.faceTestFlag = false;
    this.registerFlag = false;
    this.mge = "人脸测试开始";
    this.getMedia2();
    let times = 15;
    console.time("runTime:");
    for (let index = 0; index < times; index++) {
      //console.log(index.toString() + new Date());
      // setTimeout(this.validatorphoto,1000);
      // if(index==0)
      // {
      //     this.imgData="";
      // }
      if (index > 0) {
        this.canvasShow = false;
      }
      await this.sleep(350).then(async () => {
        // 这里写sleep之后需要去做的事情

        console.log(index.toString() + new Date());
        console.time("test" + index);
      });
      console.timeEnd("test" + index);
      console.log(index.toString() + new Date());
      //   console.log("ss"+ await this.validatorphoto(index).then());
      if (index > 0) {
        await this.validatorphoto(index)
          .then(() => {
            this.faceTestFlag = true;
          })
          .catch(function (error) {
            //alert(error );
            console.log(error);
            //return false;
          });
      }
      if (this.faceTestFlag) {
        index = 9999;
        console.timeEnd("runTime:");
        //  this.canvasShow=true;
        //  this.closeMedia();

        return;
      }
    }
    console.timeEnd("runTime:");
    this.mge = "人脸验证失败";
    // var videomask = document.getElementById('videomask');
    // videomask.style.backgroundImage ="url('')";
    // this.canvasShow=true;
    this.closeMedia();

    await this.$store.dispatch({
      type: "user/getUrl",
      id: this.userId,
    });
  }
  async faceDelete() {
    if (this.userId == 0) {
      alert("请先选择用户");
      return;
    }

    this.$Modal.confirm({
      title: this.L("Tips"),
      content: this.L("删除用户人脸数据"),
      okText: this.L("Yes"),
      cancelText: this.L("No"),
      onOk: async () => {
        let rep =await axios
          .post("http://localhost:5032/Face/FaceDelete", {
            userId: this.userId.toString(),
          })
          .then(async (response) => {
            //人脸删除成功
            // if (response.data.ret_code == 0) {
            //   await this.$store.dispatch({
            //     type: "user/deletephoto",
            //     userId: this.userId,
            //   }).then(()=>{
            //   //设置人脸图片未空
            //       this.$store.dispatch({
            //     type: "user/getUrl",
            //     id: this.userId,
            //     });
            //   });

            // //  (this.$refs.myimg as any).src=null;
            // } else {
            // }
            // // console.log(response.data.ret_code);
            // console.log(response);
          })
          .catch(function (error) {
            console.log(error);
          });
          //避免数据库中人脸图片未删除
           await this.$store.dispatch({
                type: "user/deletephoto",
                userId: this.userId,
              }).then(()=>{
              //设置人脸图片未空
                  this.$store.dispatch({
                type: "user/getUrl",
                id: this.userId,
                });
              });
      },
    });


  }

  async sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
  }

  ceftoweb() {
    alert(222);
  }

  async veinregister() {
    if (this.userId == 0) {
      alert("请先选择用户");
      return;
    }
    let veinFeatureStr:string;
    //注册六次指静脉
    for(let i=0 ; i<6 ;i++){
      this.fingermge ="第"+(i+1)+"次采集开始,请按下手指"
      await axios.post("http://localhost:5032/Vein/VeinRegisterOnce", {
            userId: this.userId.toString(),
            registerCount: i,
            veinFeature:veinFeatureStr
          })
        .then(async (response) => {
          // this.serviceConnect=false;
          // console.log(response);
          if(response.data.success == true){
            veinFeatureStr=response.data.veinFeature;
            this.fingermge = response.data.msg+",请移开手指"
            await this.sleep(2000).then(async () => {

            })
          }else{
            this.fingermge = response.data.msg+",请重新注册"
            i = 999;
            return;
          }
        })
        .catch((error) => {

            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }

        });
    }

    
    // try {
    //   // @ts-ignore：无法被执行的代码的错误
    //   veinjs.register(this.userId);
    // } catch (error) {}
    console.log('start getveins');
    //点击后延迟操作获取更新的指静脉数据
    this.sleep(3000).then(async () => {
        for (let index = 0; index < 5; index++) {
          await this.sleep(1500).then(async () => {
            // 这里写sleep之后需要去做的事情
            console.log(index.toString() + new Date());
                 await   this.$store.dispatch({
                  type: "user/getVeins",
                  userId: this.userId,
                });
            });
        }
      });

  }
  async veindelete() {
    if (this.veinId == 0) {
      alert("请先选择要删除的指静脉");
    }

    axios.post("http://localhost:5032/Vein/VeinDelete", {
            veinId: this.veinId,
          })
        .then(async (response) => {
          if (response.data.result) {

            this.fingermge = "指静脉删除成功" + response.data.result;
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
        
        await this.$store.dispatch({
          type: "user/deletevein",
          id: this.veinId,
        });

    // try {
    //   // @ts-ignore：无法被执行的代码的错误
    //   veinjs.delete(this.veinId);
    // } catch (error) {}

    this.$store.dispatch({
      type: "user/getVeins",
      userId: this.userId,
    });
  }
  veintest() {
    this.fingermge = "指静脉测试开始请平放手指";
    axios.get('http://localhost:5032/Vein/VeinMatch')
        .then(async (response) => {
          // this.serviceConnect=false;
          // console.log(response);
          if (response.data > 0) {
            this.fingermge = "指静脉测试成功" + response.data;
          } else {
            this.fingermge = "指静脉不存在,请重试";
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
  }

  getMedia2() {
    this.imgData = "";
    let video = document.getElementById("video") as any;
    let constraints = {
      //AI摄像头必须是这个分辨率，其它会不显示
      video: { width: 1280, height: 720 },
      audio: false,
    };
    let promise = navigator.mediaDevices.getUserMedia(constraints);
    promise
      .then(function (MediaStream: any) {
        MediaStreamTrack =
          typeof MediaStream.stop === "function"
            ? MediaStream
            : MediaStream.getTracks()[0];
        video.srcObject = MediaStream;
        video.play();
      })
      .catch(function (PermissionDeniedError) {
        console.log(PermissionDeniedError);
      });
  }

  takePhoto() {
    //获得Canvas对象
    let canvas = document.getElementById("canvas") as any;
    let video = document.getElementById("video") as any;
    let ctx = canvas.getContext("2d");
    ctx.drawImage(video, 0, 0, 336, 189);
    var type = "image/jpeg";
    //从画布上获取照片数据
    var imgData = canvas.toDataURL(type);
    //将图片转换为Base64
    this.imagedata = imgData.substr(23);
    //console.log("base64Data:"+imgData);
  }
  closeMedia() {
    // let video = document.getElementById("video") as any;
    // video.pause();
    MediaStreamTrack && (MediaStreamTrack as any).stop();
    // this.mge='验证结束';
  }
  imagedata: string;
  async validator() {
    let loginModel = {
      image: this.imagedata,
      rememberMe: false,
    };
    await this.$store.dispatch({
      type: "app/face",
      data: loginModel,
    });
    await this.$store.dispatch({
      type: "session/init",
    });
    console.log(this.$store.state.session.user.id);
    console.log(this.$store.state.session.user.name);
    Cookies.set("facing", "0");
    Cookies.set("userId", this.$store.state.session.user.id);
    //location.reload();
    this.$router.push({
      name: Cookies.get("last_page_name"),
    });

    console.log(Cookies.get("last_page_name"));
    console.log("validator");

    return true;
  }
  imgData: string;
  video: any;
  timerCallback() {
    //  let video = document.getElementById("video") as any;
    if (this.video.paused || this.video.ended) {
      return;
    }
    if (this.faceTestFlag || this.registerFlag) {
      return;
    }
    let canvas = document.getElementById("canvas") as any;
    let ctx = canvas.getContext("2d");
    ctx.drawImage(this.video, 0, 0, 336, 189);
    var type = "image/jpeg";
    //从画布上获取照片数据
    this.imgData = canvas.toDataURL(type);
    //   this.computeFrame();
    let self = this;
    setTimeout(function () {
      self.timerCallback();
    }, 0);
  }
  async validatorphoto(index: number): Promise<any> {
    //         //获得Canvas对象
    // let canvas = document.getElementById("canvas") as any;
    // let video = document.getElementById("video") as any;
    // var videomask = document.getElementById('videomask');
    // let ctx = canvas.getContext('2d');
    // ctx.drawImage(video, 0, 0, 336, 189);
    // var type='image/jpeg';
    // 		//从画布上获取照片数据
    // var imgData = canvas.toDataURL(type);
    // this.imagedata="";
    // videomask.style.backgroundImage ="url('"+imgData+"')";
    // if(index>2)
    // {
    // //将图片转换为Base64
    // this.imagedata = imgData.substr(23);
    // }
    //console.log(imgData);
    //this.$Message.success('任务下达成功。');
    console.time("validatorphoto" + index);
    let rep = await axios
      .post("http://localhost:5032/Face/FaceMatch", {
        imageData: this.imgData.substr(23),
      })
      .then(async (response) => {
        console.timeEnd("validatorphoto" + index);
        console.log(response);
        
        //人脸识别成功
        if (response.data> 0) {
            this.mge = "人脸测试成功" + response.data;
            Cookies.set('facing', response.data);


            this.faceTestFlag = true;
            //this.canvasShow=true;
            this.closeMedia();
            return Promise.resolve("人脸测试成功");
            

        } else {
          console.log("人脸识别失败");
          return Promise.reject("人脸识别失败");

        }

      })
      .catch(function (error) {
        console.timeEnd("validatorphoto" + index);
        console.log(error);
        return Promise.reject("人脸识别失败");

      });
  }
  async faceregister(index: number): Promise<any> {
    //         //获得Canvas对象
    // let canvas = document.getElementById("canvas") as any;
    // let video = document.getElementById("video") as any;
    // let ctx = canvas.getContext('2d');
    // ctx.drawImage(video, 0, 0, 336, 189);
    // var type='image/jpeg';
    // //从画布上获取照片数据
    // var imgData = canvas.toDataURL(type);
    // //将图片转换为Base64
    // this.imagedata = imgData.substr(23);
    //console.log("base64Data:"+imgData);
    //this.$Message.success('任务下达成功。');
    //console.time("faceregister" + index);
    let rep = await axios
      .post("http://localhost:5032/Face/FaceAdd", {
        imageData: this.imgData.substr(23),
        //user_id: this.userId,
        userId:this.userId.toString(),

      })
      .then(async (response) => {
        //console.timeEnd("faceregister" + index);
        //人脸识别成功
        //console.log(response.data.error)
        if (response.data.errno == "0") {
          //console.log(this.registerFlag)
          this.registerFlag = true;
          console.log(response.data)
          
          await this.$store.dispatch({
            type: "user/facePhotoUpload",
            data: {
              userId: this.userId,
              image: this.imgData.substr(23),
            },
          });
          this.mge = "人脸注册成功" + this.userNameSelect;
          
          this.closeMedia();
          return Promise.resolve("人脸测试成功");
          //return true;
        } else {
          //this.registerFlag = true;
          return Promise.reject("人脸注册失败");
          //return false;
        }
        // console.log(response.data.ret_code);
        //console.log(response);
      })
      .catch(function (error) {
        console.timeEnd("faceregister" + index);
        console.log(error);
        return Promise.reject("人脸注册失败");
        //return false;
      });
  }

 //20220326 添加注册前的人脸校验  待测试
  async faceregisterWithCHeck(index: number): Promise<any> {
    console.time("faceregister" + index);
    //1、获取人脸特征
    let rep = await axios
      .post("/RecognitionManage", {
        RequestValue: 2817,
        pass: "123456",
        image_type: "image",
        quality_control: "NONE",
        image_content: this.imgData.substr(23),
      })
      .then(async (response) => {
        console.timeEnd("faceregister" + index);
        //人脸获取成功
        if (response.data.ret_code == 0) {
           //2、查找设备中是否已存在该用户的人脸
              await axios.post("/RecognitionManage", {
                      RequestValue: 2819,
                      pass: "123456",
                      image_type: "image",
                      quality_control: "NONE",
                      image_content: this.imgData.substr(23),
                    })
                    .then(async (response) => {
                      console.log(response);
                      //人脸搜索成功
                      if (response.data.ret_code == 0) {
                        if (response.data.user_list[0].score > 0.8) {
                          //存在人脸信息
                          this.mge = "人脸注册失败，存在已注册人脸信息";
                          this.registerFlag = true;
                          this.closeMedia();
                          return Promise.resolve("人脸测试成功");
                        } else {
                          //不存在人脸信息
                          this.faceregister(index);
                        }
                      } else {
                        //不存在人脸信息
                        this.faceregister(index);
                      }
                    })
                    .catch(function (error) {
                      return Promise.reject("人脸识别失败");
                    });
          //return true;
        } else {
          return Promise.reject("没有获取到人脸");
        }
      })
      .catch(function (error) {
        console.timeEnd("faceregister" + index);
        console.log(error);
        return Promise.reject("人脸注册失败，接口调用失败");
      });
  }
  async getpage() {
    this.pagerequest.maxResultCount = 8;
    this.pagerequest.skipCount = (this.currentPage - 1) * 8;
    //filters

    if (this.creationTime.length > 0) {
      this.pagerequest.from = this.creationTime[0];
    }
    if (this.creationTime.length > 1) {
      this.pagerequest.to = this.creationTime[1];
    }

    await this.$store.dispatch({
      type: "user/getAllByClient",
      data: this.pagerequest,
    });
  }
  async getpage2() {
    this.getpage()
    this.pageChange(1)
  }
  get pageSize() {
    return this.$store.state.user.pageSize;
  }
  get totalCount() {
    return this.$store.state.user.totalCount;
  }
  get currentPage() {
    return this.$store.state.user.currentPage;
  }
  columns = [
    {
      title: this.L("Id"),
      key: "id",
    },
    {
      title: this.L("用户名"),
      key: "userName",
    },
    {
      title: this.L("姓名"),
      key: "name",
    },
  ];

  columns2 = [
    {
      title: this.L("指静脉ID"),
      key: "id",
    },
    {
      title: this.L("用户ID"),
      key: "userId",
    },
    {
      title: this.L("数据"),
      render: (h: any, params: any) => {
        return h("span", "data....");
      },
    },
  ];
  async created() {
    // changeTitle();
    this.getpage();
     await this.$store.dispatch({
                type:'user/getRoles'
    })
  }
  async mounted() {
    this.video = document.getElementById("video") as any;
    this.video.addEventListener("play", this.timerCallback, false);
  }
  async destroyed() {
    this.closeMedia();
  }
  openKeyboard()
        {
                try {
                    console.log("openkeyboard");
                    //打开键盘
                // @ts-ignore：无法被执行的代码的错误
                veinjs.openosk();
                } catch (error) {}
        }
}
</script>
<style scoped  lang="less">
.page-body {
  height: 800px;
}
#myimg {
  height: 126px;
  width: 224px;
}
.imgdiv {
  margin-top: 10%;
  margin-left: 10%;
}
#video {
  position: absolute;
  height: 189px;
  width: 336px;
}
#canvas {
  position: absolute;
  z-index: 980;
}

.videodiv {
  height: 209px;
  width: 366px;
  // z-index: 999;
}
.demo-split {
  height: 670px;
  border: 1px solid #dcdee2;
  margin: 10px;
}
.demo-split-pane {
  padding: 5px;
}
.buttondiv1 {
  margin-left: 10%;
}
.buttondiv2 {
  margin-left: 15%;
}
.topvideoleft {
  position: absolute;
  height: 189px;
  width: 100px;
  z-index: 999;
  //  background-color:rgba(27, 26, 26,0.5);
}
.topvideoright {
  position: absolute;
  left: 236px;
  height: 189px;
  width: 100px;
  z-index: 999;
  //  background-color:rgba(27, 26, 26,0.5);
}
.videomask {
  position: absolute;
  height: 189px;
  width: 339px;
  z-index: 999;
  background-color: #fff;
  background-repeat: no-repeat;
}
//     //video 隐藏  好像不起作用
//     video::-webkit-media-controls {
//   display:none !important;
// }
</style>