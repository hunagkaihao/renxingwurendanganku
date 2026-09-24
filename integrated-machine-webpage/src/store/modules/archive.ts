import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Archive from '../entities/archive'
import Recordlist from '../entities/recordlist'
import User from '../entities/user'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

interface ArchiveState extends ListState<Archive>{
    editArchive:Archive;
    bindingArchive:Archive;
    users:User[];
    recordlists:Recordlist[]
}
class ArchiveModule extends ListModule<ArchiveState,any,Archive>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Archive>(),
        users: new Array<User>(),
        loading:false,
        editArchive:new Archive(),
        bindingArchive:new Archive(),
        recordlists:new Array<Recordlist>()
    }
    actions={
        async getAll(context:ActionContext<ArchiveState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/Archives/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Archive>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<ArchiveState,any>,payload:any){
            await Ajax.post('/api/services/app/Archives/Create',payload.data);
        },
        async update(context:ActionContext<ArchiveState,any>,payload:any){
            await Ajax.put('/api/services/app/Archives/Update',payload.data);
        },
        async delete(context:ActionContext<ArchiveState,any>,payload:any){
            await Ajax.delete('/api/services/app/Archives/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<ArchiveState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/Archives/Get?Id='+payload.id);
            return reponse.data.result as Archive;
        },
        async binding(context:ActionContext<ArchiveState,any>,payload:any){
            await Ajax.post('/api/services/app/Archives/SetRfid',payload.data);
        },
        async getlists(context:ActionContext<ArchiveState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/Archives/Get?Id='+payload.id);
            context.state.loading=false;
            context.state.list=reponse.data.result;
        }, async getUsers(context:ActionContext<ArchiveState,any>){
            let reponse=await Ajax.get('/api/services/app/User/GetAll');
            context.state.users=reponse.data.result.items as User[];
        },
        async getrecordLists(context:ActionContext<ArchiveState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/Archives/GetRecordListsByGoodsId?mainId='+payload.id);
            context.state.recordlists=reponse.data.result.items as Recordlist[];
        }
        
    };
    mutations={
        setCurrentPage(state:ArchiveState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:ArchiveState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:ArchiveState,archive:Archive){
            state.editArchive=archive;
        },
        bind(state:ArchiveState,archive:Archive){
            state.bindingArchive=archive;
        }
    }
}
const archiveModule=new ArchiveModule();
export default archiveModule;