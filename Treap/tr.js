class Node {
    constructor(key, prior) {
        this.key = key;
        this.prior = prior;
        this.l = null;
        this.r = null;
    }
}

function split(t, key, l, r) {
    if (!t) {
        l = null;
        r = null;
    } else if (t.key <= key) {
        split(t.r, key, t.r, r);
        l = t;
    } else {
        split(t.l, key, l, t.l);
        r = t;
    }
}

function insert(t, it) {
    if (!t) {
        t = it;
    } else if (it.prior > t.prior) {
        split(t, it.key, it.l, it.r);
        t = it;
    } else {
        insert(t.key <= it.key ? t.r : t.l, it);
    }
}
let f = new Node(1,9);
insert(f,new Node(-1,8));
insert(f,new Node(0,6));
insert(f,new Node(-3,5));
insert(f,new Node(-2,4));
insert(f,new Node(3,8));
insert(f,new Node(2,7));
insert(f,new Node(4,5));

let l= null,r= null;
split(f,3,l,r);
console.log(l)