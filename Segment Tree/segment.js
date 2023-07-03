let A = [1,2,3,4];
let t = Array.from({length:A.length*4},()=>0)
function build(a, v, tl,tr) {
    if (tl === tr) {
        t[v] = a[tl];
    } else {
        let tm = Math.floor((tl + tr) / 2);
        build(a, v*2, tl, tm);
        build(a, v*2+1, tm+1, tr);
        t[v] = t[v*2] + t[v*2+1];
    }
}
function sum(x,v,tl,tr,l,r){
    if(l>r) return 0;
    if(tl === l && tr === r) return t[v];
    let m = Math.floor((tl+tr)/2);
    return sum(x,v*2,     tl ,m  ,l               ,Math.min(r,m))+sum(x,v*2+1,m+1,tr ,Math.max(l,m+1) ,r)
}
build([...A],1,0,A.length-1);
console.log(sum(A,1,0,A.length-1,0,2))


// Faster segment trees
t = Array.from({ length : 2 * A.length+1 },()=>0);
for(let i = 0;i < A.length; ++i) t[A.length+i] = A[i];
t.build = ()=>{
    for(let i = A.length-1;i>0;--i ){
        t[i] = t[2 * i] + t[2 * i + 1];
    }
}
t.update = (i,v)=>{
    i = i + A.length;
    t[i] = v;
    while(i>1){
        t[i>>1] = t[i] + t[i^1];
        i>>=1;
    }
}
t.query = (l,r)=>{
    let n = A.length;
    l += n;
    r += n;
    let res = 0;
    while(l<r){
        if(l&1){
            res += t[l++];
        }
        if(r&1){
            res += t[--r];
        }
        l>>=1;
        r>>=1;
    }
    return res;
}
t.build();
console.log(t.query(0,2));