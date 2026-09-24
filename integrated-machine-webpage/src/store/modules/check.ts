import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Check from '../entities/check'
import Checklist from '../entities/checklist'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

interface CheckState extends ListState<Check>{
    editCheck:Check;
    checklists:Checklist[]
}
class CheckModule extends ListModule<CheckState,any,Check>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Check>(),
        loading:false,
        editCheck:new Check(),
        checklists:new Array<Checklist>()
    }
    actions={
        async getAll(context:ActionContext<CheckState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/CheckMains/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Check>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<CheckState,any>,payload:any){
            await Ajax.post('/api/services/app/CheckMains/Create',payload.data);
        },
        async update(context:ActionContext<CheckState,any>,payload:any){
            await Ajax.put('/api/services/app/CheckMains/Update',payload.data);
        },
        async delete(context:ActionContext<CheckState,any>,payload:any){
            await Ajax.delete('/api/services/app/CheckMains/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<CheckState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/CheckMains/Get?Id='+payload.id);
            return reponse.data.result as Check;
        },
        async getLists(context:ActionContext<CheckState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/CheckMains/GetCheckLists?mainId='+payload.id);
            context.state.checklists=reponse.data.result.items as Checklist[];
        },
        async createCheck(context:ActionContext<CheckState,any>,payload:any){
            await Ajax.post('/api/services/app/CheckMains/CreateCheck');
        },
        async executeCheck(context:ActionContext<CheckState,any>,payload:any){
            await Ajax.post('/api/services/app/CheckMains/ExecuteCheck?checkId='+payload.checkId);
        } 
    };
    mutations={
        setCurrentPage(state:CheckState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:CheckState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:CheckState,check:Check){
            state.editCheck=check;
        }
    }
}
const checkModule=new CheckModule();
export default checkModule;