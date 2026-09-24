import {Store,Module,ActionContext} from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import Cell from '../entities/cell'
import CabinetInitialData from '../entities/cabinetInitialData'
import Ajax from '../../lib/ajax'
import PageResult from '@/store/entities/page-result';
import ListMutations from './list-mutations'
import StorageMain from '../entities/archivebox'

interface CellState extends ListState<Cell>{
    editCell:Cell;
    maxZ:number;//排
    maxX:number;//列
    maxY:number;//层
    maxJie:number;//最大节数
    jieCounts:number;//节中库位数
    stations:Cell[];
    storageMain:StorageMain
}
class CellModule extends ListModule<CellState,any,Cell>{
    state={
        totalCount:0,
        currentPage:1,
        pageSize:10,
        list: new Array<Cell>(),
        loading:false,
        editCell:new Cell(),
        maxX:0,
        maxY:0,
        maxZ:0,
        maxJie:0,
        jieCounts:0,
        stations:new Array<Cell>(),
        storageMain:new StorageMain()
    }
    actions={
        async getAll(context:ActionContext<CellState,any>,payload:any){
            context.state.loading=true;
            let reponse=await Ajax.get('/api/services/app/Cells/GetAll',{params:payload.data});
            context.state.loading=false;
            let page=reponse.data.result as PageResult<Cell>;
            context.state.totalCount=page.totalCount;
            context.state.list=page.items;
        },
        async create(context:ActionContext<CellState,any>,payload:any){
            await Ajax.post('/api/services/app/Cells/Create',payload.data);
        },
        async update(context:ActionContext<CellState,any>,payload:any){
            await Ajax.put('/api/services/app/Cells/Update',payload.data);
        },
        async delete(context:ActionContext<CellState,any>,payload:any){
            await Ajax.delete('/api/services/app/Cells/Delete?Id='+payload.data.id);
        },
        async get(context:ActionContext<CellState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/Cells/Get?Id='+payload.id);
            return reponse.data.result as Cell;
        },
        async batIn(context:ActionContext<CellState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/ManageMains/BatBoxIn',payload.data);
        },
        async lock(context:ActionContext<CellState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/Cells/LockCell?cellId='+payload.id);
        },
        async unlock(context:ActionContext<CellState,any>,payload:any){
            let reponse=await Ajax.post('/api/services/app/Cells/UnLockCell?cellId='+payload.id);
        },
        async getcellmax(context:ActionContext<CellState,any>,payload:any){
            let reponse=await Ajax.get('/api/services/app/DAClient/GetInitialData');
            let initdata=reponse.data.result as CabinetInitialData;
            context.state.maxX=initdata.initialColumns;
            context.state.maxY=initdata.initialLayers;
            context.state.maxZ=initdata.initialRows;
            context.state.maxJie=initdata.initialParts;
            context.state.jieCounts=initdata.initialMinColumns;
        },
        async getStations(context:ActionContext<CellState,any>,payload:any){
            let reponse= await Ajax.get('/api/services/app/Cells/GetStations');
            context.state.stations=reponse.data.result as Cell[];
        },
        //通过一个库位的出库任务
        async getBoxByCellId(context:ActionContext<CellState,any>,payload:any){
            let reponse= await Ajax.get('api/services/app/StorageMains/GetBoxByCellId?cellId='+payload.data)
            context.state.storageMain = reponse.data.result as StorageMain;
        }         
    };
    mutations={
        setCurrentPage(state:CellState,page:number){
            state.currentPage=page;
        },
        setPageSize(state:CellState,pagesize:number){
            state.pageSize=pagesize;
        },
        edit(state:CellState,cell:Cell){
            state.editCell=cell;
        }
    }
}
const cellModule=new CellModule();
export default cellModule;