#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace CLRLib {

    public ref class NewsSorter
    {
    private:
        String^ keyword;

    public:
        NewsSorter(String^ keyword)
        {
            this->keyword = keyword;
        }

        int Evaluate(String^ title, String^ summary, String^ contains)
        {
            int num = 0;
            if (title != nullptr && title->Contains(keyword))
            {
                num += 4;
            }
            if (summary != nullptr && summary->Contains(keyword))
            {
                num += 2;
            }
            if (contains != nullptr && contains->Contains(keyword))
            {
                num += 1;
            }
            return num;
        }

        List<int>^ SortNews(
            List<int>^ ids,
            List<String^>^ titles,
            List<String^>^ summaries,
            List<String^>^ contents)
        {
            List<Tuple<int, int>^>^ evaluatedNews = gcnew List<Tuple<int, int>^>();

            for (int i = 0; i < ids->Count; i++)
            {
                int score = Evaluate(titles[i], summaries[i], contents[i]);
                evaluatedNews->Add(Tuple::Create(score, ids[i]));
            }

            evaluatedNews->Sort(gcnew Comparison<Tuple<int, int>^>(
                this, &NewsSorter::CompareScores));

            List<int>^ sortedIds = gcnew List<int>();
            for each (auto item in evaluatedNews)
            {
                sortedIds->Add(item->Item2);
            }

            return sortedIds;
        }

    private:
        int CompareScores(Tuple<int, int>^ a, Tuple<int, int>^ b)
        {
            return b->Item1.CompareTo(a->Item1); // Descending order
        }
    };
}
