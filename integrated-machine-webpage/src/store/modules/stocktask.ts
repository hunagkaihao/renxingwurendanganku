import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Stocktask from '../entities/stocktask'
import Stocktasklist from '../entities/stocktasklist'
import Cell from '../entities/cell'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

class TaskFaultDto {
    controlTaskId: number;
    deviceTaskId: number;
    manageTaskId: number;
    barcode: string;
    taskFaultDesc: string;
  }

interface StocktaskState extends ListState<Stocktask>{
    editStocktask:Stocktask;
    stocktask:Stocktask;
    stocktasklists:Stocktasklist[];
    cells:Cell[];
    queueCount:number;
    taskFaultlists:TaskFaultDto[];
    alertCount:number
    stockoutPagetype:string
}

class StocktaskModule extends ListModule<StocktaskState,any,Stocktask>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Stocktask>(),
        loading:false,
        editStocktask:new Stocktask(),
        stocktask:new Stocktask(),
        stocktasklists:new Array<Stocktasklist>(),
        cells:new Array<Cell>(),
        queueCount:0,
        taskFaultlists:new Array<TaskFaultDto>(),
        alertCount:0,
        stockoutPagetype:"Search"
    }
    actions={
        async getAll(context:ActionContext<StocktaskState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/ManageMains/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Stocktask>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/Create',payload.data);
        },
        async update(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.put('/api/services/app/ManageMains/Update',payload.data);
        },
        async delete(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.delete('/api/services/app/ManageMains/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/ManageMains/Get?Id='+payload.id);
            return reponse.data.result as Stocktask;
        },
        async getTask(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/ManageMains/GetTaskByRfid?Rfid='+payload.rfid);
            context.state.stocktask= reponse.data.result as Stocktask;
        },
        async getCTaskIn(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/ManageMains/GetCurrentTaskIn?excuteFlag='+payload.excuteFlag);
            context.state.stocktask= reponse.data.result as Stocktask;
        },
        async getCTaskOut(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/ManageMains/GetCurrentTaskOut?excuteFlag='+payload.excuteFlag);
            context.state.stocktask= reponse.data.result as Stocktask;
        },
        async getLists(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/ManageLists/GetManageLists?mainId='+payload.mainId);
            context.state.stocktasklists=reponse.data.result.items as Stocktasklist[];
        },
        async taskAssign(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskAssign?mainId='+payload.mainId);
        },
        async taskAssignUseRfid(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/taskAssignUseRfid?rfid='+payload.rfid);
        },
        async taskAssignCell(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskAssignWithCell',payload.data);
        },
        async taskUpdateInCell(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskUpdateWithInCell',payload.data);
        },
        async taskUpdateOutCell(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskUpdateWithOutCell',payload.data);
        },
        async taskAssignOut(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/OutTaskAssign?mainId='+payload.mainId);
        },       
        async taskConfirm(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskOutConfirm',payload.data);
        },
        async taskConfirmBack(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskOutConfirmBack',payload.data);
        },
        async taskCancelIn(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/ManageCancelIn?mainId='+payload.mainId);
        },
        async taskAssignBatIn(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TaskAssignBatIn?planId='+payload.planId);
        },
        async taskComplete(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/ManageComplete?mainId='+payload.mainId);
        },
        async getCells(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse= await Ajax.get('/api/services/app/Cells/GetCellByZY',{params:payload.data});
            context.state.cells=reponse.data.result as Cell[];
        },        
        async tasklistCheckInt(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/TasklistCheckInt?mainId='+payload.mainId);
        },        
        async openDoor(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/ManageMains/ControlDoorOpen?mId='+payload.mId);
        },        
        async taskAutoAssign(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse= await Ajax.post('/api/services/app/ManageMains/TaskAutoAssign');
            context.state.queueCount=reponse.data.result as number;
        },        
        async taskAutoAssignOut(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse= await Ajax.post('/api/services/app/ManageMains/AutoOutTaskAssign');
            context.state.queueCount=reponse.data.result as number;
        },
        //获取WCS异常任务  
        async getWcsTaskException(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse= await Ajax.get('/api/services/app/DAClient/GetWcsTaskException');
            context.state.taskFaultlists=reponse.data.result.items as TaskFaultDto[];
            context.state.alertCount=context.state.taskFaultlists.length;
            return reponse;
        },
        //重新发送WCS任务
        async redoWcsTask(context:ActionContext<StocktaskState,any>,payload:any){
            await Ajax.post('/api/services/app/DAClient/RedoWcsTask',payload.data);
        },
        //获取取档方式
        async getSettingValue(context:ActionContext<StocktaskState,any>,payload:any){
            let reponse = await Ajax.get('/api/services/app/DAClient/GetSettingValue?name=StockOutPageType');
            context.state.stockoutPagetype = reponse.data.result as string;
        },
    };
    mutations={
        setCurrentPage(state:StocktaskState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:StocktaskState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:StocktaskState,stocktask:Stocktask){
            state.editStocktask=stocktask;
        }
    }
}
const stocktaskModule=new StocktaskModule();
export default stocktaskModule;