let A = [1, 3, 5, 7, 9, 11];
let t = Array.from({ length: A.length *2 +1 }, () => 0);
let lz = [...t];
function build(a, v, tl, tr) {
    if (tl === tr) {
        t[v] = a[tl];
    } else {
        let tm = Math.floor((tl + tr) / 2);
        build(a, v * 2 +1, tl, tm);
        build(a, v * 2 + 2, tm + 1, tr);
        t[v] = t[v * 2 +1 ] + t[v * 2 + 2];
    }
}

function sum(x, v, tl, tr, l, r) {
    if (l > r) return 0;
    if (tl === l && tr === r) return t[v];
    let m = Math.floor((tl + tr) / 2);
    return (
        sum(x, v * 2, tl, m, l, Math.min(r, m)) +
        sum(x, v * 2 + 1, m + 1, tr, Math.max(l, m + 1), r)
    );
}

function lazyUpdate(idx, hl, hr, l, r, v) {
    if (lz[idx] !== 0) {
        t[idx] += (hr - hl + 1) * lz[idx];
        if (hl !== hr) {
            lz[idx * 2 +1] += lz[idx];
            lz[idx * 2 + 2] += lz[idx];
        }
        lz[idx] = 0;
    }
    if (hl > r || hr < l || hl > hr) return;
    if (hl >= l && hr <= r) {
        t[idx] += (hr - hl + 1) * v;
        if (hl !== hr) {
            lz[idx * 2 +1] += v;
            lz[idx * 2 + 2] += v;
        }
        return;
    }
    let m = Math.floor((hl + hr) / 2);
    lazyUpdate(idx * 2+1 , hl, m, l, r, v);
    lazyUpdate(idx * 2 + 2, m + 1, hr, l, r, v);
    t[idx] = t[idx * 2+1] + t[idx * 2 + 2];
}

function lazyQuery(idx, hl, hr, l, r) {
    if (lz[idx] !== 0) {
        t[idx] += (hr - hl + 1) * lz[idx];
        if (hl !== hr) {
            lz[idx * 2 +1] += lz[idx];
            lz[idx * 2 + 2] += lz[idx];
        }
        lz[idx] = 0;
    }
    if (hl > r || hr < l || hl > hr) return 0;
    if (hl >= l && hr <= r) return t[idx];
    let m = Math.floor((hl + hr) / 2);
    return (
        lazyQuery(idx * 2 +1, hl, m, l, r) +
        lazyQuery(idx * 2 + 2, m + 1, hr, l, r)
    );
}

build([...A], 0, 0, A.length - 1);
lazyUpdate(0, 0, A.length-1, 0, A.length - 1, 2);
lazyUpdate(0, 0, A.length-1, 0, A.length - 1, 1);
console.log(lazyQuery(0, 1, A.length-1, 0, A.length - 1));

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
t.query = (l,r)=> {
    let n = A.length;
    l += n;
    r += n;
    let res = 0;
    while (l < r) {
        if (l & 1) {
            res += t[l++];
        }
        if (r & 1) {
            res += t[--r];
        }
        l >>= 1;
        r >>= 1;
    }
    return res;
}
t.lazyUpdate = (idx,hl,hr,l,h,v)=>{

}
