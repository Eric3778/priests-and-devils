#include <bits/stdc++.h>
using namespace std;

bool visited[4][4][2]; //标志一个状态是否被展开过 

bool is_valid(int ld, int lp){ //判断一个状态是否违反规则（某一边恶魔大于牧师） 
	if((lp!=0 && ld > lp) || ((3-lp!=0) && (3-ld)> (3-lp)))return false;
	return true;
}

stack<pair<int, int> > search(int ld, int lp, int side){ //输入一个状态，进行寻路 
	visited[ld][lp][side] = true;
	stack<pair<int, int> > path;
	if(ld==3 && lp==3){  //结束状态，用全0标记 
		path.push(pair<int, int>(0,0));
		return path;
	}
	int D = (side==0)?ld:(3-ld);
	int P = (side==0)?lp:(3-lp);
	int next_side = (side==0)?1:0;
	for(int d = 0; d <= D; d++){
		for(int p = 0; p <= P; p++){
			if(d+p>2 || d+p==0)continue;
			int temp_ld = (side==0)?ld-d:ld+d;
			int temp_lp = (side==0)?lp-p:lp+p;
			if(!is_valid(temp_ld, temp_lp))continue; //检查变换之后是否违反规则 
			if(visited[temp_ld][temp_lp][next_side])continue; //检测变换之后的状态是否已经展开过 
			printf("(%d, %d, %d)->(%d, %d, %d)\n",ld,lp,side,temp_ld,temp_lp,next_side);
			path = search(temp_ld, temp_lp, next_side); //递归查找一条路径 
			if(path.size()!=0){ //如果返回结果为空，说明是死路 
				path.push(pair<int,int>(d,p));
				return path;
			}
		}
	}
	return path;
}

int main(){
	memset(visited, false, sizeof(visited));
	stack<pair<int, int> > path;
	path = search(0,0,1); //左边没有Devil和Priest，船在右边作为起始状态 
	int curr_side = 1;
	while(path.size()!=0){
		pair<int, int> move = path.top();
		path.pop();
		curr_side = (curr_side==1)?0:1;
		if(move.first==0 && move.second==0){
			cout<<"success!"<<endl; 
			break;
		}
		printf("move %d Devils and %d Priests ", move.first,move.second);
		string move_str = (curr_side==0)?"from right to left":"from left to right";
		cout<<move_str<<endl;
	}
} 
