// Minimum Stack

let x = Object.create(null)
Object.defineProperty(x,"val",{
    value:[],
    writable: true
})
Object.defineProperty(x,"getSmallest",{
    value:function (){return this.val[this.val.length-1].min}
})
Object.defineProperty(x,"push",{
    value:function (y){
        if(this.val.length === 0) this.val.push({ele:y,min:y});
        else this.val.push({ele:y,min:Math.min(y,this.val[this.val.length-1].min)});
    }
})
Object.defineProperty(x,"pop",{
    value:function (){
        this.val.pop();
    }
})
x.push(10)
x.push(5)
x.push(11)
x.push(4)
x.pop();
console.log(x.getSmallest())

//Minimun Stack using IIFE
let MStack = (function (){
    let val = [];
    return {
        get : ()=>{
            return val[val.length-1].min;
        },
        push:(y)=>{
            if(val.length === 0) val.push({ele:y,min:y});
            else val.push({ele:y,min:Math.min(y,val[val.length-1].min)});
        },
        pop:()=>{
            val.pop();
        }
    }
})();
MStack.push(2);
MStack.push(1);
MStack.push(21);
MStack.push(0);
console.log(MStack.get())
MStack.pop();
console.log(MStack.get())

//Minimum Stack using contructor

const MStack_ = (function (){
    let val = [];
    return {
        get : ()=>{
            return val[val.length-1].min;
        },
        push:(y)=>{
            if(val.length === 0) val.push({ele:y,min:y});
            else val.push({ele:y,min:Math.min(y,val[val.length-1].min)});
        },
        pop:()=>{
            val.pop();
        }
    }
});
MStack = new MStack_();
MStack.push(2);
MStack.push(1);
MStack.push(21);
MStack.push(0);
console.log(MStack.get())
MStack.pop();
console.log(MStack.get())

//Minimum Stack using Class

class MS{
    #val;
    constructor() {
        this.#val = []
    }
    get min(){
        return this.#val[this.#val.length-1].min;
    }
    set el(y){
        if(this.#val.length === 0) this.#val.push({ele:y,min:y});
        else this.#val.push({ele:y,min:Math.min(y,this.#val[this.#val.length-1].min)});
    }
}
let m = new MS();
m.el = 2;
m.el = 1;
m.el = 3;
m.el = 33;
console.log(m.min)
