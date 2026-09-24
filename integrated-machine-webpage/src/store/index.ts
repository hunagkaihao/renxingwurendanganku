import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);
import app from './modules/app'
import session from './modules/session'
import account from './modules/account'
import user from './modules/user'
import role from './modules/role'
import tenant from './modules/tenant'
import archive from './modules/archive'
import archivebox from './modules/archivebox'
import archiveboxlist from './modules/archiveboxlist'
import stocktask from './modules/stocktask'
import cell from './modules/cell'
import rfid from './modules/rfid'
import record from './modules/record'
import check from './modules/check'
import checkhis from './modules/checkhis'
import board from './modules/board'
const store = new Vuex.Store({
    state: {
        //
    },
    mutations: {
        //
    },
    actions: {

    },
    modules: {
        app,
        session,
        account,
        user,
        role,
        tenant,
        archive,
        archivebox,
        archiveboxlist,
        stocktask,
        cell,
        rfid,
        record,
        check,
        checkhis,
        board
    }
});

export default store;