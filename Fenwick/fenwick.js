let A = [1,2,3,4,5,6];
const RSB = (x)=>x&-x;
function create(a){
    let fen = [0,...a];
    for(let i = 1;i<=a.length;i++){
        let parent = i + RSB(i);
            if(parent<=a.length) fen[parent] += fen[i];
    }
    return fen;
}

let x = create(A);
function sum(z){
    let sums = 0;
    let i = z;
    while(i>0){
        sums += x[i];
        i-=RSB(i)
    }
    return sums;
}
function sum_l_r(l,r){
    ++r;
    return sum(r) - sum(l);
}
function update(i,v){
    while(i<=A.length){
        x[i] += v;
        i+=RSB(i);
    }
}
update(1,5);
console.log(sum(2))