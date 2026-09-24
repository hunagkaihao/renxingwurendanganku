import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Archivebox from '../entities/archivebox'
import Archiveboxlist from '../entities/archiveboxlist'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'

interface ArchiveboxState extends ListState<Archivebox>{
    success:boolean;
    editArchiveBox:Archivebox;
    bindingArchiveBox:Archivebox;
    bindrfidArchiveBox:Archivebox;
    archiveboxlists:Archiveboxlist[]
}
class ArchiveboxModule extends ListModule<ArchiveboxState,any,Archivebox>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Archivebox>(),
        loading:false,
        success:false,
        editArchiveBox:new Archivebox(),
        bindingArchiveBox:new Archivebox(),
        bindrfidArchiveBox:new Archivebox(),
        archiveboxlists:new Array<Archiveboxlist>()
    }
    actions={
        async getAll(context:ActionContext<ArchiveboxState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/StorageMains/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Archivebox>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<ArchiveboxState,any>,payload:any){
            await Ajax.post('/api/services/app/StorageMains/Create',payload.data);
        },
        async update(context:ActionContext<ArchiveboxState,any>,payload:any){
            await Ajax.put('/api/services/app/StorageMains/Update',payload.data);
        },
        async delete(context:ActionContext<ArchiveboxState,any>,payload:any){
            await Ajax.delete('/api/services/app/StorageMains/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<ArchiveboxState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/StorageMains/Get?Id='+payload.id);
            return reponse.data.result as Archivebox;
        },
        async binding(context:ActionContext<ArchiveboxState,any>,payload:any){
            await Ajax.post('/api/services/app/StorageMains/BindingRfid',payload.data);
        },
        async getLists(context:ActionContext<ArchiveboxState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/StorageLists/GetStorageLists?mainId='+payload.id);
            context.state.archiveboxlists=reponse.data.result.items as Archiveboxlist[];
        },
        async bindingRfid(context:ActionContext<ArchiveboxState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/StorageMains/BindingArchives',payload.data);
            context.state.archiveboxlists=reponse.data.result.items as Archiveboxlist[];
        },
        async fullIn(context:ActionContext<ArchiveboxState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/ManageMains/FullInTaskByRfid?Rfid='+payload.Rfid);
            context.state.success=true;
        },
        async boxEmptyOutTask(context:ActionContext<ArchiveboxState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/ManageMains/BoxEmptyOutTask?stgId='+payload.stgId);
            context.state.success=true;
        }
        
    };
    mutations={
        setCurrentPage(state:ArchiveboxState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:ArchiveboxState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:ArchiveboxState,archivebox:Archivebox){
            state.editArchiveBox=archivebox;
        },
        bind(state:ArchiveboxState,archivebox:Archivebox){
            state.bindingArchiveBox=archivebox;
        },
        bindrfid(state:ArchiveboxState,archivebox:Archivebox){
            state.bindrfidArchiveBox=archivebox;
        }

    }
}
const archiveboxModule=new ArchiveboxModule();
export default archiveboxModule;