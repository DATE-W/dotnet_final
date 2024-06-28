// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"

//BOOL APIENTRY DllMain( HMODULE hModule,
//                       DWORD  ul_reason_for_call,
//                       LPVOID lpReserved
//                     )
//{
//    switch (ul_reason_for_call)
//    {
//    case DLL_PROCESS_ATTACH:
//    case DLL_THREAD_ATTACH:
//    case DLL_THREAD_DETACH:
//    case DLL_PROCESS_DETACH:
//        break;
//    }
//    return TRUE;
//}

#include <iostream>
#include <string>
#include <algorithm>
#include <stdio.h>
#include "stdlib.h"
#include <tchar.h>

struct Posts
{
    std::string title;
    std::string contains;
    int approvalNum;
    int favouriteNum;
};

extern "C" __declspec(dllexport) int CountKeywordOccurrences(const std::string & text, const std::string & keyword)
{
    int count = 0;
    size_t index = text.find(keyword);
    while (index != std::string::npos)
    {
        count++;
        index = text.find(keyword, index + keyword.length());
    }
    return count;
}

extern "C" __declspec(dllexport) double CalculateRelevance(const Posts& post, const std::string& keyword)
{
    int titleKeywordCount = CountKeywordOccurrences(post.title, keyword);
    int contentKeywordCount = CountKeywordOccurrences(post.contains, keyword);

    // 这里的权重是经验性的，可以根据需要进行调整
    double titleWeight = 0.7;
    double contentWeight = 0.3;
    double approvalWeight = 0.1;
    double favoriteWeight = 0.05;

    // 计算关联度分数
    double relevanceScore =
        titleWeight * titleKeywordCount +
        contentWeight * contentKeywordCount +
        approvalWeight * post.approvalNum +
        favoriteWeight * post.favouriteNum;

    return relevanceScore;
}


