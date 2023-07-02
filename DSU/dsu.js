/**
 * Union set algorithm, inverse Ackermann function is complexity
 */
class DSU{
    #parent;
    #size;
    constructor() {
        this.#parent = [];
        this.#size = [];
    }
    getEle(){ return this.#parent; }
    make(v){
        if(this.#parent[v]) return;
        this.#parent[v] = v;
        this.#size[v] = 1;
    }
    find(v){
        if(v === this.#parent[v]) return v;
        return this.#parent[v] = this.find(this.#parent[v]);
    }
    union(v,u){
        v = this.find(v);
        u = this.find(u);
        if(v !== u){
            if(this.#size[v] < this.#size[u]) [v,u] = [u,v];
            this.#parent[u] = v;
            this.#size[v] += this.#size[u];
        }
    }
}
let d = new DSU();

// Connected components in graph
let edges = [[1, 0], [2, 3], [3, 4],[55,66],[66,77]];
edges.forEach(x=>{
    d.make(x[0]);
    d.make(x[1]);
    d.union(x[0],x[1]);
})
let s = new Set();
edges.forEach(x=>{
    s.add(d.find(x[0]))
});
console.log(s.size)

function gn(x,i,j){
    if(i<0 || j<0 || i>=x.length || j >= x[i].length) return null;
    else return x[i][j];
}

d = new DSU();


