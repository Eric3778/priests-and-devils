#include <bits/stdc++.h>
using namespace std;

bool visited[4][4][2]; //��־һ��״̬�Ƿ�չ���� 

bool is_valid(int ld, int lp){ //�ж�һ��״̬�Ƿ�Υ������ĳһ�߶�ħ������ʦ�� 
	if((lp!=0 && ld > lp) || ((3-lp!=0) && (3-ld)> (3-lp)))return false;
	return true;
}

stack<pair<int, int> > search(int ld, int lp, int side){ //����һ��״̬������Ѱ· 
	visited[ld][lp][side] = true;
	stack<pair<int, int> > path;
	if(ld==3 && lp==3){  //����״̬����ȫ0��� 
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
			if(!is_valid(temp_ld, temp_lp))continue; //���任֮���Ƿ�Υ������ 
			if(visited[temp_ld][temp_lp][next_side])continue; //���任֮���״̬�Ƿ��Ѿ�չ���� 
			printf("(%d, %d, %d)->(%d, %d, %d)\n",ld,lp,side,temp_ld,temp_lp,next_side);
			path = search(temp_ld, temp_lp, next_side); //�ݹ����һ��·�� 
			if(path.size()!=0){ //������ؽ��Ϊ�գ�˵������· 
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
	path = search(0,0,1); //���û��Devil��Priest�������ұ���Ϊ��ʼ״̬ 
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
