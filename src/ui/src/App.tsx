import React, { useMemo, useState } from "react";
import "./App.scss";
import { useQuery } from "@tanstack/react-query";
import Icon from "@mdi/react";
import { mdiChevronLeft, mdiChevronRight } from "@mdi/js";

interface Quote {
  id: number;
  text: string;
  source: string;
}

function App() {
  const {
    isLoading: isQuoteDataLoading,
    error: quoteFetchError,
    data: quoteData,
  } = useQuery<Quote[]>({
    queryKey: ["quotes"],
    queryFn: () =>
      fetch(`${process.env.REACT_APP_API_URL}/api/quote`).then((res) =>
        res.json()
      ),
  });

  const [index, setIndex] = useState(0);

  const setIndexValue = (i: number) => {
    if (!quoteData) {
      return;
    }
    if (i < 0) {
      setIndex(quoteData.length - 1);
    } else if (i >= quoteData.length) {
      setIndex(0);
    } else {
      setIndex(i);
    }
  };

  const quote = useMemo(() => {
    if (!quoteData) {
      return undefined;
    }
    return quoteData[index];
  }, [quoteData, index]);

  const renderedSection = useMemo(() => {
    if (isQuoteDataLoading) {
      return <div className="spinner-border text-primary" role="status"></div>;
    }
    if (quoteFetchError || !quote) {
      return (
        <div className="alert alert-danger" role="alert">
          Error retrieving quotes.
        </div>
      );
    }
    return (
      <blockquote className="blockquote">
        <p>{quote?.text}</p>
        <footer className="blockquote-footer">{quote?.source}</footer>
      </blockquote>
    );
  }, [quote, isQuoteDataLoading, quoteFetchError]);

  return (
    <div className="container text-center">
      <div className="row">
        <div className="col">{renderedSection}</div>
      </div>
      {!!quote && (
        <div className="row">
          <div className="col">
            <div className="btn-group" role="group" aria-label="Basic example">
              <button
                type="button"
                className="btn btn-primary"
                onClick={() => setIndexValue(index - 1)}
              >
                <Icon path={mdiChevronLeft} size={1} title="Previous Quote" />
              </button>
              <button
                type="button"
                className="btn btn-primary"
                onClick={() => setIndexValue(index + 1)}
              >
                <Icon path={mdiChevronRight} size={1} title="Previous Quote" />
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
