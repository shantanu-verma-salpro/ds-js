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