class MQ{
    #val;#a;#r;
    constructor() {
        this.#val = [];
        this.#a = this.#r = 0;
    }

    push(x){
        while(this.#val.length && this.#val[this.#val.length-1].ele > x) this.#val.pop();
        this.#val.push({ele:x,idx:this.#a});
        ++this.#a;
    }
    delete(){
        if(this.#val.length<1) return;
        if(this.#val.length && this.#val[0].idx === this.#r) this.#val.shift();
        ++this.#r;
    }
    min(){
        return this.#val[0]?.ele;
    }
}
const M = new MQ();

// Finding the minimum for all subarrays of fixed length
const arr = [5,0,6,1,7,2];
const m  = 3;
let i = 0;
Array.prototype.slice.call(arr,i,m).forEach(x=>M.push(x));
i = m;
while(i<arr.length){
    console.log(M.min());
    M.push(arr[i]);
    M.delete();
    ++i;
}
console.log(M.min());