import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Checkhis from '../entities/checkhis'
import Checklisthis from '../entities/checklisthis'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

interface CheckhisState extends ListState<Checkhis>{
    editCheckhis:Checkhis;
    checklisthiss:Checklisthis[]
}
class CheckhisModule extends ListModule<CheckhisState,any,Checkhis>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Checkhis>(),
        loading:false,
        editCheckhis:new Checkhis(),
        checklisthiss:new Array<Checklisthis>()
    }
    actions={
        async getAll(context:ActionContext<CheckhisState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/CheckMainHiss/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Checkhis>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<CheckhisState,any>,payload:any){
            await Ajax.post('/api/services/app/CheckMainHiss/Create',payload.data);
        },
        async update(context:ActionContext<CheckhisState,any>,payload:any){
            await Ajax.put('/api/services/app/CheckMainHiss/Update',payload.data);
        },
        async delete(context:ActionContext<CheckhisState,any>,payload:any){
            await Ajax.delete('/api/services/app/CheckMainHiss/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<CheckhisState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/CheckMainHiss/Get?Id='+payload.id);
            return reponse.data.result as Checkhis;
        },
        async getLists(context:ActionContext<CheckhisState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/CheckMainHiss/GetCheckLists?mainId='+payload.id);
            context.state.checklisthiss=reponse.data.result.items as Checklisthis[];
        },
        async lossconfirm(context:ActionContext<CheckhisState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/CheckMainHiss/InventoryLossConfirm?checkListHisId='+payload.id);
        },
        async checkconfirm(context:ActionContext<CheckhisState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/CheckMainHiss/InventoryConfirm?checkListHisId='+payload.id);
        },
        async plancomplete(context:ActionContext<CheckhisState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/CheckMainHiss/CheckPlanComplete?checkCode='+payload.id);
        }
    };
    mutations={
        setCurrentPage(state:CheckhisState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:CheckhisState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:CheckhisState,checkhis:Checkhis){
            state.editCheckhis=checkhis;
        }
    }
}
const checkhisModule=new CheckhisModule();
export default checkhisModule;