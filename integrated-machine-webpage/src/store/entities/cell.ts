import Entity from './entity'
export default class Cell extends Entity<number>{
    cellCode:string;
    cellName:string;  
    cellStatus:string; 
}