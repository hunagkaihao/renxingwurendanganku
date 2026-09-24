import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Rfid from '../entities/rfid'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

interface RfidState extends ListState<Rfid>{
    editRfid:Rfid;
}
class RfidModule extends ListModule<RfidState,any,Rfid>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Rfid>(),
        loading:false,
        editRfid:new Rfid()
    }
    actions={
        async getAll(context:ActionContext<RfidState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/Rfids/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Rfid>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<RfidState,any>,payload:any){
            await Ajax.post('/api/services/app/Rfids/Create',payload.data);
        },
        async update(context:ActionContext<RfidState,any>,payload:any){
            await Ajax.put('/api/services/app/Rfids/Update',payload.data);
        },
        async delete(context:ActionContext<RfidState,any>,payload:any){
            await Ajax.delete('/api/services/app/Rfids/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<RfidState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/Rfids/Get?Id='+payload.id);
            return reponse.data.result as Rfid;
        }        
    };
    mutations={
        setCurrentPage(state:RfidState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:RfidState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:RfidState,rfid:Rfid){
            state.editRfid=rfid;
        }
    }
}
const rfidModule=new RfidModule();
export default rfidModule;